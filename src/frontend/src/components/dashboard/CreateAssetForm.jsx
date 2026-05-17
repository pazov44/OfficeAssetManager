import React from 'react';

function CreateAssetForm({ 
  newAsset, 
  setNewAsset, 
  handleCreateAsset, 
  isMobile, 
  labelStyle, 
  inputStyle 
}) {
  return (
    <form onSubmit={handleCreateAsset} style={{ background: 'white', padding: '24px', borderRadius: '12px', marginBottom: '32px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)', border: '1px solid #e2e8f0' }}>
      <h3 style={{ margin: '0 0 16px 0', fontSize: '15px', color: '#475569', fontWeight: '600' }}>Register New Corporate Asset</h3>
      <div style={{ display: 'grid', gridTemplateColumns: isMobile ? '1fr' : 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', alignItems: 'flex-end' }}>
        <div>
          <label style={labelStyle}>Asset Name</label>
          <input type="text" value={newAsset.name} onChange={e => setNewAsset({...newAsset, name: e.target.value})} required style={inputStyle} placeholder="e.g. Dell Precision"/>
        </div>
        <div>
          <label style={labelStyle}>Asset Tag</label>
          <input type="text" value={newAsset.assetTag} onChange={e => setNewAsset({...newAsset, assetTag: e.target.value})} required style={inputStyle} placeholder="e.g. TAG-00412"/>
        </div>
        <div>
          <label style={labelStyle}>Serial Number</label>
          <input type="text" value={newAsset.serialNumber} onChange={e => setNewAsset({...newAsset, serialNumber: e.target.value})} required style={inputStyle} placeholder="e.g. S/N-CN08X"/>
        </div>
        <div>
          <label style={labelStyle}>Category</label>
          <input type="text" value={newAsset.category} onChange={e => setNewAsset({...newAsset, category: e.target.value})} required placeholder="e.g. Hardware" style={inputStyle}/>
        </div>
      </div>
      <div style={{ marginTop: '16px', display: 'flex', gap: '16px', flexDirection: isMobile ? 'column' : 'row', alignItems: isMobile ? 'stretch' : 'flex-end' }}>
        <div style={{ flex: 1 }}>
          <label style={labelStyle}>Description</label>
          <input type="text" value={newAsset.description} onChange={e => setNewAsset({...newAsset, description: e.target.value})} required style={inputStyle} placeholder="Add location details..."/>
        </div>
        <button type="submit" style={{ padding: '0 24px', backgroundColor: '#10b981', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600', height: '40px', fontSize: '14px' }}>Create</button>
      </div>
    </form>
  );
}

export default CreateAssetForm;