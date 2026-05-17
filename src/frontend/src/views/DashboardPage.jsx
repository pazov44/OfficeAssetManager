import { useState, useEffect } from 'react';
import { assetService } from '../services/assetService';
import { reservationService } from '../services/reservationService';
import { logService } from '../services/logService';

import Header from '../components/layout/Header';
import AssetCard from '../components/dashboard/AssetCard';
import CreateAssetForm from '../components/dashboard/CreateAssetForm'; 
import ReservationsTable from '../components/dashboard/ReservationsTable';
import LogsViewer from '../components/dashboard/LogsViewer';

function DashboardPage() {
  const [activeTab, setActiveTab] = useState('assets');
  const [assets, setAssets] = useState([]);
  const [myReservations, setMyReservations] = useState([]);
  const [allReservations, setAllReservations] = useState([]);
  const [logs, setLogs] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const [selectedAsset, setSelectedAsset] = useState(null);
  const [selectedAssetLogs, setSelectedAssetLogs] = useState([]);
  const [viewModalOpen, setViewModalOpen] = useState(false);

  const [newAsset, setNewAsset] = useState({ name: '', assetTag: '', serialNumber: '', category: '', description: '' });
  const [editingAssetId, setEditingAssetId] = useState(null);
  const [editAssetPayload, setEditAssetPayload] = useState({ name: '', assetTag: '', serialNumber: '', category: '', description: '' });
  const [bookingStates, setBookingStates] = useState({});
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);

  useEffect(() => {
    const handleResize = () => setIsMobile(window.innerWidth < 768);
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  const token = localStorage.getItem('token');
  const parseJwt = (token) => { try { return JSON.parse(atob(token.split('.')[1])); } catch (e) { return {}; } };
  const userPayload = parseJwt(token);
  const isAdmin = userPayload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] === 'Admin' || userPayload.role === 'Admin';

  useEffect(() => { loadData(); }, [activeTab]);

  const loadData = async () => {
    try {
      setError(''); setLoading(true);
      if (activeTab === 'assets') setAssets(await assetService.getAllAssets() || []);
      else if (activeTab === 'my-bookings') setMyReservations(await reservationService.getMyReservations() || []);
      else if (activeTab === 'all-bookings' && isAdmin) setAllReservations(await reservationService.getAllReservations() || []);
      else if (activeTab === 'logs' && isAdmin) setLogs(await logService.getAllActivityLogs() || []);
    } catch (err) { setError(err.response?.data?.message || 'Failed to fetch tracking data pipeline.'); } finally { setLoading(false); }
  };

  const handleInspectAsset = async (id) => {
    try {
      setError(''); 
      setLoading(true);
      
      if (isAdmin) {
        const [assetDetails, assetSubLogs, legacyLogRef] = await Promise.all([
          assetService.getAssetById(id), 
          assetService.getLogsByAssetId(id), 
          logService.getLogsForAssetId(id)
        ]);
        
        setSelectedAsset(assetDetails);
        
        const uniqueLogsMap = new Map();
        [...(assetSubLogs || []), ...(legacyLogRef || [])].forEach(log => log?.id && uniqueLogsMap.set(log.id, log));
        setSelectedAssetLogs(Array.from(uniqueLogsMap.values()));
      } else {
        const assetDetails = await assetService.getAssetById(id);
        setSelectedAsset(assetDetails);
        setSelectedAssetLogs([]); 
      }
      
      setViewModalOpen(true);
    } catch (err) { 
      setError('Failed to fetch asset metrics telemetry specifications.'); 
    } finally { 
      setLoading(false); 
    }
  };

  const handleCreateAsset = async (e) => {
    e.preventDefault();
    try {
      await assetService.createAsset(newAsset);
      setNewAsset({ name: '', assetTag: '', serialNumber: '', category: 'Hardware', description: '' }); loadData();
    } catch (err) { setError(err.response?.data?.message || 'Asset creation blocked.'); }
  };

  const handleStartEdit = (asset) => {
    setEditingAssetId(asset.id);
    setEditAssetPayload({ name: asset.name, assetTag: asset.assetTag, serialNumber: asset.serialNumber, category: asset.category || 'Hardware', description: asset.description || '' });
  };

  const handleUpdateAssetSubmit = async (e, id) => {
    e.preventDefault();
    try { await assetService.updateAsset(id, editAssetPayload); setEditingAssetId(null); loadData(); } catch { setError('Failed to update target.'); }
  };

  const handleDeleteAsset = async (id) => {
    if (!window.confirm('Delete this asset permanently?')) return;
    try { setError(''); setLoading(true); await assetService.deleteAsset(id); setAssets(prev => prev.filter(a => a.id !== id)); } 
    catch (err) { if (err.response?.status === 404) setAssets(prev => prev.filter(a => a.id !== id)); else setError(err.response?.data?.message || 'Failed to delete asset'); } finally { setLoading(false); }
  };

  const handleBookAsset = async (e, assetId) => {
    e.preventDefault(); const config = bookingStates[assetId] || {};
    if (!config.startDate || !config.endDate) return alert('Both tracking dates are mandatory.');
    const start = new Date(config.startDate), end = new Date(config.endDate), today = new Date(); today.setHours(0,0,0,0);
    if (start < today) return alert('Start Date cannot be set in the past.');
    if (end < start) return alert('Return Date cannot be earlier than Start Date.');
    try {
      setError(''); await reservationService.createReservation({ assetId: parseInt(assetId, 10), startDate: start.toISOString(), endDate: end.toISOString() });
      setBookingStates(prev => ({ ...prev, [assetId]: { startDate: '', endDate: '' } })); alert('Reservation registered successfully!'); setActiveTab('my-bookings');
    } catch (err) { const msg = err.response?.data?.message || 'Booking conflict.'; setError(msg); alert(`Server Error: ${msg}`); }
  };

  const handleDateChange = (assetId, field, value) => setBookingStates(prev => ({ ...prev, [assetId]: { ...(prev[assetId] || {}), [field]: value } }));
  const handleUpdateStatus = async (id, newStatus) => { try { setError(''); await reservationService.updateStatus(id, newStatus); loadData(); } catch { setError('Failed to update request state.'); } };
  const handleCancelBooking = async (id) => { try { setError(''); await reservationService.cancelReservation(id); loadData(); } catch { setError('Failed to revoke allocation.'); } };

  const labelStyle = { display: 'block', fontSize: '12px', fontWeight: '600', color: '#475569', marginBottom: '4px' };
  const tinyLabelStyle = { display: 'block', fontSize: '10px', fontWeight: '700', color: '#64748b', marginBottom: '4px', letterSpacing: '0.05em' };
  const inputStyle = { width: '100%', padding: '8px 12px', borderRadius: '6px', border: '1px solid #cbd5e1', fontSize: '14px', boxSizing: 'border-box' };
  const compactInputStyle = { width: '100%', padding: '6px 10px', borderRadius: '4px', border: '1px solid #cbd5e1', fontSize: '13px', boxSizing: 'border-box' };
  const tableStyle = { width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '14px', minWidth: '600px' };
  const thStyle = { padding: '12px 16px', fontWeight: '600', color: '#475569', borderBottom: '1px solid #e2e8f0' };
  const tdStyle = { padding: '12px 16px', color: '#334155', borderBottom: '1px solid #f1f5f9' };
  const navLinkStyle = (isActive) => ({ padding: '8px 16px', backgroundColor: isActive ? '#3b82f6' : 'transparent', color: isActive ? 'white' : '#94a3b8', border: 'none', borderRadius: '6px', fontSize: '14px', cursor: 'pointer', fontWeight: '600', transition: 'all 0.15s ease', width: isMobile ? '100%' : 'auto', textAlign: 'left' });
  const badgeStyle = (status) => { const isApp = status === 'Approved', isRej = status === 'Rejected'; return { padding: '4px 8px', borderRadius: '4px', fontSize: '12px', fontWeight: '600', backgroundColor: isApp ? '#d1fae5' : isRej ? '#fee2e2' : '#fef3c7', color: isApp ? '#065f46' : isRej ? '#991b1b' : '#92400e' }; };
  const todayString = new Date().toISOString().split('T')[0];

  return (
    <div style={{ minHeight: '100vh', width: '100%', fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif', backgroundColor: '#f3f4f6', margin: 0, padding: 0 }}>
      <Header userPayload={userPayload} isAdmin={isAdmin} activeTab={activeTab} setActiveTab={setActiveTab} isMobile={isMobile} navLinkStyle={navLinkStyle} />
      <main style={{ width: '100%', padding: isMobile ? '16px 12px' : '32px 24px', boxSizing: 'border-box' }}>
        {error && <div style={{ padding: '16px', backgroundColor: '#fef2f2', color: '#991b1b', borderRadius: '8px', marginBottom: '28px', borderLeft: '4px solid #ef4444', fontSize: '14px', fontWeight: '500' }}>⚠️ {error}</div>}
        
        {activeTab === 'assets' && (
          <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: isMobile ? 'flex-start' : 'center', marginBottom: '24px', flexDirection: isMobile ? 'column' : 'row', gap: '8px' }}>
              <h2 style={{ color: '#0f172a', margin: 0, fontSize: '24px', fontWeight: '700' }}>Assets</h2>
              <span style={{ color: '#64748b', fontSize: '14px', fontWeight: '500' }}>{assets.length} Assets Available</span>
            </div>
            
            {isAdmin && (
              <CreateAssetForm 
                newAsset={newAsset} 
                setNewAsset={setNewAsset} 
                handleCreateAsset={handleCreateAsset} 
                isMobile={isMobile} 
                labelStyle={labelStyle} 
                inputStyle={inputStyle} 
              />
            )}

            {loading ? <p style={{ color: '#64748b', textAlign: 'center', padding: '40px' }}>Syncing telemetry loops with server cluster...</p> : assets.length === 0 ? <div style={{ background: 'white', padding: '60px 24px', borderRadius: '12px', textAlign: 'center', border: '2px dashed #cbd5e1' }}><p style={{ color: '#475569', fontSize: '16px', fontWeight: '600', margin: 0 }}>No assets are available.</p></div> : (
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '24px' }}>
                {assets.map(asset => (
                  <AssetCard key={asset.id} asset={asset} isAdmin={isAdmin} isMobile={isMobile} editingAssetId={editingAssetId} editAssetPayload={editAssetPayload} setEditAssetPayload={setEditAssetPayload} handleUpdateAssetSubmit={handleUpdateAssetSubmit} setEditingAssetId={setEditingAssetId} handleStartEdit={handleStartEdit} handleDeleteAsset={handleDeleteAsset} handleInspectAsset={handleInspectAsset} handleBookAsset={handleBookAsset} bookingStates={bookingStates} handleDateChange={handleDateChange} tinyLabelStyle={tinyLabelStyle} compactInputStyle={compactInputStyle} todayString={todayString} />
                ))}
              </div>
            )}
          </div>
        )}

        {activeTab === 'my-bookings' && (<div><h2 style={{ color: '#0f172a', marginBottom: '24px', fontSize: '24px', fontWeight: '700' }}>My Reservations</h2>{myReservations.length === 0 ? <div style={{ background: 'white', padding: '40px', borderRadius: '12px', textAlign: 'center', border: '1px solid #e2e8f0' }}><p style={{ color: '#64748b', margin: 0 }}>You have no reservations made.</p></div> : <ReservationsTable reservations={myReservations} showAdminActions={false} handleCancelBooking={handleCancelBooking} tableStyle={tableStyle} thStyle={thStyle} tdStyle={tdStyle} badgeStyle={badgeStyle} currentUser={userPayload} /> }</div>)}
        {activeTab === 'all-bookings' && isAdmin && (<div><h2 style={{ color: '#0f172a', marginBottom: '24px', fontSize: '24px', fontWeight: '700' }}>All Reservations</h2>{allReservations.length === 0 ? <div style={{ background: 'white', padding: '40px', borderRadius: '12px', textAlign: 'center', border: '1px solid #e2e8f0' }}><p style={{ color: '#64748b', margin: 0 }}>No reservations are available.</p></div> : <ReservationsTable reservations={allReservations} showAdminActions={true} handleUpdateStatus={handleUpdateStatus} tableStyle={tableStyle} thStyle={thStyle} tdStyle={tdStyle} badgeStyle={badgeStyle} currentUser={userPayload} /> }</div>)}
        {activeTab === 'logs' && isAdmin && (<div><h2 style={{ color: '#0f172a', marginBottom: '24px', fontSize: '24px', fontWeight: '700' }}>Asset Logs</h2>{logs.length === 0 ? <div style={{ background: 'white', padding: '40px', borderRadius: '12px', textAlign: 'center', border: '1px solid #e2e8f0' }}><p style={{ color: '#64748b', margin: 0 }}>No actions on assets have been made.</p></div> : <LogsViewer logs={logs} isMobile={isMobile} /> }</div>)}
      </main>

      {viewModalOpen && selectedAsset && (
        <div style={{ position: 'fixed', top: 0, left: 0, right: 0, bottom: 0, backgroundColor: 'rgba(15, 23, 42, 0.6)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 100, padding: isMobile ? '10px' : '20px' }}>
          <div style={{ backgroundColor: 'white', width: '100%', maxWidth: '600px', borderRadius: '12px', boxShadow: '0 20px 25px -5px rgba(0,0,0,0.1)', overflow: 'hidden', display: 'flex', flexDirection: 'column', maxHeight: '95vh' }}>
            <div style={{ padding: '20px', backgroundColor: '#1e293b', color: 'white', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}><h3 style={{ margin: 0, fontSize: '18px', fontWeight: '600' }}>Asset Details</h3><button onClick={() => setViewModalOpen(false)} style={{ background: 'transparent', border: 'none', color: '#94a3b8', fontSize: '20px', cursor: 'pointer' }}>&times;</button></div>
            <div style={{ padding: isMobile ? '16px' : '24px', overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <div><span style={tinyLabelStyle}>Asset Name</span><div style={{ fontSize: '20px', fontWeight: '700', color: '#0f172a' }}>{selectedAsset.name}</div></div>
              <div style={{ display: 'grid', gridTemplateColumns: isMobile ? '1fr' : '1fr 1fr', gap: '12px' }}>
                <div><span style={tinyLabelStyle}>Asset Tag</span><div style={{ fontFamily: 'monospace', fontSize: '14px', backgroundColor: '#f8fafc', padding: '8px', borderRadius: '4px', border: '1px solid #e2e8f0' }}>{selectedAsset.assetTag}</div></div>
                <div><span style={tinyLabelStyle}>Serial Number</span><div style={{ fontFamily: 'monospace', fontSize: '14px', backgroundColor: '#f8fafc', padding: '8px', borderRadius: '4px', border: '1px solid #e2e8f0' }}>{selectedAsset.serialNumber}</div></div>
              </div>
              <div><span style={tinyLabelStyle}>Description</span><p style={{ margin: 0, fontSize: '14px', color: '#334155', backgroundColor: '#f8fafc', padding: '12px', borderRadius: '6px', border: '1px solid #e2e8f0', lineHeight: 1.5 }}>{selectedAsset.description || 'No execution metadata found for this object entity configuration.'}</p></div>
              
              {/* HIDDEN LOGS DISPLAY CONDITIONALLY FOR NON-ADMIN USERS */}
              {isAdmin && (
                <div>
                  <span style={tinyLabelStyle}>Logs History ({selectedAssetLogs.length})</span>
                  <div style={{ maxHeight: '200px', overflowY: 'auto', border: '1px solid #e2e8f0', borderRadius: '6px', backgroundColor: '#fafafa' }}>
                    {selectedAssetLogs.length === 0 ? (
                      <div style={{ padding: '16px', fontSize: '13px', color: '#94a3b8', textAlign: 'center' }}>No historical security events mapped to this token.</div>
                    ) : (
                      selectedAssetLogs.map((log, index) => (
                        <div key={log.id || index} style={{ padding: '10px 12px', borderBottom: index === selectedAssetLogs.length - 1 ? 'none' : '1px solid #e2e8f0', fontSize: '12px' }}><span style={{ color: '#3b82f6', fontFamily: 'monospace' }}>[{new Date(log.createdAt || log.timestamp).toLocaleDateString()}]</span> <strong>{log.action}</strong> by <span style={{ color: '#64748b' }}>{log.performedBy || 'System'}</span>{log.details && <div style={{ color: '#64748b', marginTop: '2px', fontStyle: 'italic' }}>{log.details}</div>}</div>
                      ))
                    )}
                  </div>
                </div>
              )}
            </div>
            <div style={{ padding: '16px 24px', backgroundColor: '#f8fafc', borderTop: '1px solid #e2e8f0', display: 'flex', justifyContent: 'flex-end' }}><button onClick={() => setViewModalOpen(false)} style={{ padding: '8px 16px', backgroundColor: '#1e293b', color: 'white', border: 'none', borderRadius: '6px', fontSize: '13px', fontWeight: '600', cursor: 'pointer', width: isMobile ? '100%' : 'auto' }}>Close</button></div>
          </div>
        </div>
      )}
    </div>
  );
}

export default DashboardPage;