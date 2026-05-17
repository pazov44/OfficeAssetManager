import React from 'react';

function LogsViewer({ logs, isMobile }) {
  return (
    <div style={{ background: 'white', padding: isMobile ? '16px' : '24px', borderRadius: '12px', maxHeight: '650px', overflowY: 'auto', border: '1px solid #e2e8f0' }}>
      {logs.map(log => (
        <div key={log.id} style={{ padding: '14px 0', borderBottom: '1px solid #f1f5f9', fontSize: '14px', display: 'flex', flexDirection: 'column', gap: '4px' }}>
          <div style={{ display: 'flex', flexDirection: isMobile ? 'column' : 'row', justifyContent: 'space-between', gap: isMobile ? '4px' : 0 }}>
            <div>
              <span style={{ color: '#3b82f6', fontWeight: '700', fontFamily: 'monospace', marginRight: '8px' }}>[{new Date(log.createdAt || log.timestamp).toLocaleString()}]</span>
              <strong style={{ color: '#0f172a' }}>{log.action}</strong> by <span style={{ color: '#475569', fontStyle: 'italic' }}>{log.performedBy || 'System Context'}</span>
            </div>
            <span style={{ fontSize: '12px', color: '#94a3b8', fontWeight: '500', alignSelf: isMobile ? 'flex-start' : 'center' }}>Asset Entity ID: #{log.assetId}</span>
          </div>
          {log.details && <div style={{ fontSize: '13px', color: '#64748b', backgroundColor: '#f8fafc', padding: '8px 12px', borderRadius: '6px', marginTop: '4px', borderLeft: '3px solid #cbd5e1' }}>{log.details}</div>}
        </div>
      ))}
    </div>
  );
}

export default LogsViewer;