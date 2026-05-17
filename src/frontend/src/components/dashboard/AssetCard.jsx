import React from 'react';

function AssetCard({ 
  asset, 
  isAdmin, 
  isMobile, 
  editingAssetId, 
  editAssetPayload, 
  setEditAssetPayload, 
  handleUpdateAssetSubmit, 
  setEditingAssetId, 
  handleStartEdit, 
  handleDeleteAsset, 
  handleInspectAsset, 
  handleBookAsset, 
  bookingStates, 
  handleDateChange, 
  tinyLabelStyle, 
  compactInputStyle, 
  todayString 
}) {
  const currentAssetBooking = bookingStates[asset.id] || { startDate: '', endDate: '' };
  const isEditing = editingAssetId === asset.id;

  return (
    <div style={{ background: 'white', borderRadius: '12px', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.05)', border: '1px solid #e2e8f0', display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
      
      {isEditing ? (
        /* PUT UPDATE MODE FORM */
        <form onSubmit={(e) => handleUpdateAssetSubmit(e, asset.id)} style={{ padding: '20px', flexGrow: 1, display: 'flex', flexDirection: 'column', gap: '10px' }}>
          <h4 style={{ margin: 0, fontSize: '14px', color: '#1e293b' }}>Modify Node Parameters</h4>
          <div>
            <label style={tinyLabelStyle}>NAME</label>
            <input type="text" style={compactInputStyle} value={editAssetPayload.name} onChange={e => setEditAssetPayload({...editAssetPayload, name: e.target.value})} required />
          </div>
          <div style={{ display: 'flex', gap: '8px' }}>
            <div style={{ flex: 1 }}>
              <label style={tinyLabelStyle}>TAG</label>
              <input type="text" style={compactInputStyle} value={editAssetPayload.assetTag} onChange={e => setEditAssetPayload({...editAssetPayload, assetTag: e.target.value})} required />
            </div>
            <div style={{ flex: 1 }}>
              <label style={tinyLabelStyle}>SERIAL</label>
              <input type="text" style={compactInputStyle} value={editAssetPayload.serialNumber} onChange={e => setEditAssetPayload({...editAssetPayload, serialNumber: e.target.value})} required />
            </div>
          </div>
          <div>
            <label style={tinyLabelStyle}>CATEGORY</label>
            <input type="text" style={compactInputStyle} value={editAssetPayload.category} onChange={e => setEditAssetPayload({...editAssetPayload, category: e.target.value})} />
          </div>
          <div>
            <label style={tinyLabelStyle}>DESCRIPTION</label>
            <textarea style={{ ...compactInputStyle, height: '60px', resize: 'none' }} value={editAssetPayload.description} onChange={e => setEditAssetPayload({...editAssetPayload, description: e.target.value})} />
          </div>
          <div style={{ display: 'flex', gap: '8px', marginTop: '6px' }}>
            <button type="submit" style={{ flex: 1, padding: '6px', backgroundColor: '#3b82f6', color: 'white', border: 'none', borderRadius: '4px', fontSize: '12px', fontWeight: '600', cursor: 'pointer' }}>Commit Changes</button>
            <button type="button" onClick={() => setEditingAssetId(null)} style={{ flex: 1, padding: '6px', backgroundColor: '#94a3b8', color: 'white', border: 'none', borderRadius: '4px', fontSize: '12px', fontWeight: '600', cursor: 'pointer' }}>Cancel</button>
          </div>
        </form>
      ) : (
        /* DEFAULT STANDALONE RENDER VIEW */
        <div style={{ padding: '20px', flexGrow: 1 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '8px' }}>
            <h4 style={{ margin: 0, color: '#0f172a', fontSize: '18px', fontWeight: '700' }}>{asset.name}</h4>
            <span style={{ fontSize: '11px', backgroundColor: '#f1f5f9', color: '#475569', padding: '4px 8px', borderRadius: '4px', fontWeight: '600' }}>{asset.category || 'Hardware'}</span>
          </div>
          <div style={{ fontSize: '12px', color: '#64748b', display: 'flex', flexWrap: 'wrap', gap: '8px', marginBottom: '12px', fontFamily: 'monospace' }}>
            <span><strong>TAG:</strong> {asset.assetTag}</span>
            <span style={{ display: isMobile ? 'none' : 'inline' }}>|</span>
            <span><strong>S/N:</strong> {asset.serialNumber}</span>
          </div>
          <p style={{ fontSize: '14px', color: '#334155', margin: '0 0 16px 0', lineHeight: '1.5' }}>{asset.description || 'No execution descriptions provided for this unit reference.'}</p>
          
          <button onClick={() => handleInspectAsset(asset.id)} style={{ padding: '6px 12px', backgroundColor: '#f1f5f9', color: '#1e293b', border: '1px solid #e2e8f0', borderRadius: '6px', fontSize: '12px', cursor: 'pointer', fontWeight: '600', display: 'inline-flex', alignItems: 'center', gap: '4px', width: isMobile ? '100%' : 'auto', justifyContent: isMobile ? 'center' : 'flex-start' }}>
            🔍 Asset Details & Logs
          </button>
        </div>
      )}
      
      <div style={{ padding: '20px', backgroundColor: '#f8fafc', borderTop: '1px solid #e2e8f0' }}>
        <form onSubmit={(e) => handleBookAsset(e, asset.id)} style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
          <div style={{ display: 'flex', gap: '8px' }}>
            <div style={{ flex: 1 }}>
              <label style={tinyLabelStyle}>START DATE</label>
              <input type="date" required min={todayString} value={currentAssetBooking.startDate || ''} style={compactInputStyle} onChange={e => handleDateChange(asset.id, 'startDate', e.target.value)} />
            </div>
            <div style={{ flex: 1 }}>
              <label style={tinyLabelStyle}>RETURN DATE</label>
              <input type="date" required min={currentAssetBooking.startDate || todayString} value={currentAssetBooking.endDate || ''} style={compactInputStyle} onChange={e => handleDateChange(asset.id, 'endDate', e.target.value)} />
            </div>
          </div>
          <button type="submit" style={{ padding: '10px', backgroundColor: '#3b82f6', color: 'white', border: 'none', borderRadius: '6px', fontSize: '13px', cursor: 'pointer', fontWeight: '600' }}>Create A Reservation</button>
        </form>
        
        {isAdmin && !isEditing && (
          <div style={{ display: 'flex', gap: '8px', marginTop: '8px' }}>
            <button type="button" onClick={() => handleStartEdit(asset)} style={{ flex: 1, padding: '8px', backgroundColor: 'transparent', color: '#3b82f6', border: '1px solid #bfdbfe', borderRadius: '6px', fontSize: '12px', cursor: 'pointer', fontWeight: '600' }}>
              Edit Details
            </button>
            <button onClick={() => handleDeleteAsset(asset.id)} style={{ flex: 1, padding: '8px', backgroundColor: 'transparent', color: '#ef4444', border: '1px solid #fee2e2', borderRadius: '6px', fontSize: '12px', cursor: 'pointer', fontWeight: '600' }}>
              Remove Asset
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default AssetCard;