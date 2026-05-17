import React from 'react';
import { authService } from '../../services/authService';

function Header({ userPayload, isAdmin, activeTab, setActiveTab, isMobile, navLinkStyle }) {
  return (
    <header style={{ backgroundColor: '#1e293b', color: '#f8fafc', boxShadow: '0 4px 6px -1px rgba(0,0,0,0.1)', position: 'sticky', top: 0, zIndex: 50, width: '100%' }}>
      <div style={{ 
        width: '100%',
        padding: isMobile ? '16px' : '16px 24px', 
        minHeight: '70px', 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'space-between',
        flexDirection: isMobile ? 'column' : 'row',
        gap: '16px',
        boxSizing: 'border-box'
      }}>
        
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', width: isMobile ? '100%' : 'auto' }}>
          <span style={{ fontSize: '24px' }}>🛡️</span>
          <div>
            <h1 style={{ margin: 0, fontSize: '18px', fontWeight: '700', letterSpacing: '-0.025em' }}>AssetManager</h1>
            <p style={{ margin: 0, fontSize: '11px', color: '#94a3b8' }}>Enterprise Resource Planning</p>
          </div>
        </div>

        <nav style={{ 
          display: 'flex', 
          gap: '4px', 
          height: '100%', 
          alignItems: 'center',
          flexDirection: isMobile ? 'column' : 'row',
          width: isMobile ? '100%' : 'auto'
        }}>
          <button onClick={() => setActiveTab('assets')} style={navLinkStyle(activeTab === 'assets')}>📦 Assets</button>
          <button onClick={() => setActiveTab('my-bookings')} style={navLinkStyle(activeTab === 'my-bookings')}>📅 My Reservations</button>
          {isAdmin && (
            <>
              <button onClick={() => setActiveTab('all-bookings')} style={navLinkStyle(activeTab === 'all-bookings')}>🛠️ Manage Reservations</button>
              <button onClick={() => setActiveTab('logs')} style={navLinkStyle(activeTab === 'logs')}>📋 Asset Logs</button>
            </>
          )}
        </nav>

        <div style={{ 
          display: 'flex', 
          alignItems: 'center', 
          gap: '16px',
          justifyContent: isMobile ? 'space-between' : 'flex-end',
          width: isMobile ? '100%' : 'auto',
          borderTop: isMobile ? '1px solid #334155' : 'none',
          paddingTop: isMobile ? '12px' : 0
        }}>
          <div style={{ textAlign: isMobile ? 'left' : 'right' }}>
            <div style={{ fontSize: '13px', fontWeight: '600' }}>{userPayload.unique_name || 'Signed In As'}</div>
            <span style={{ fontSize: '11px', color: isAdmin ? '#f87171' : '#60a5fa', fontWeight: '700' }}>
              {isAdmin ? 'Administrator' : 'Employee Partner'}
            </span>
          </div>
          <button onClick={() => { authService.logout(); window.location.href = '/'; }} style={{ padding: '8px 14px', backgroundColor: '#ef4444', color: 'white', border: 'none', borderRadius: '6px', fontSize: '13px', cursor: 'pointer', fontWeight: '600' }}>
            Sign Out
          </button>
        </div>
      </div>
    </header>
  );
}

export default Header;