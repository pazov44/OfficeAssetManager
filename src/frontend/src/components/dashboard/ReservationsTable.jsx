import React from 'react';

function ReservationsTable({ 
  reservations, 
  showAdminActions, 
  handleUpdateStatus, 
  handleCancelBooking, 
  tableStyle, 
  thStyle, 
  tdStyle, 
  badgeStyle,
  currentUser
}) {
  return (
    <div style={{ overflowX: 'auto', backgroundColor: 'white', borderRadius: '12px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)', border: '1px solid #e2e8f0' }}>
      <table style={tableStyle}>
        <thead>
          <tr>
            <th style={thStyle}>Asset</th>
            <th style={thStyle}>User Email</th>
            <th style={thStyle}>Duration</th>
            <th style={thStyle}>Status</th>
            <th style={thStyle}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {reservations.map((res) => {
            const isFinalState = res.status === 'Cancelled' || res.status === 'Approved' || res.status === 'Rejected';
            const isPending = res.status === 'Pending';

            const jwtEmail = currentUser?.email || 
                 currentUser?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || 
                 currentUser?.unique_name ||
                 currentUser?.name ||
                 currentUser?.username ||
                 "admin@gmail.com";

            const cleanDisplayEmail = (res.userEmail && res.userEmail !== 'N/A') ? res.userEmail : jwtEmail;

            return (
              <tr key={res.id}>
                <td style={tdStyle}>
                  <strong>{res.assetName || `Asset #${res.assetId}`}</strong>
                </td>
                
                {/* Render cleaned-up fallback email variable */}
                <td style={tdStyle}>{cleanDisplayEmail}</td>
                
                <td style={tdStyle}>
                  {new Date(res.startDate).toLocaleDateString()} - {new Date(res.endDate).toLocaleDateString()}
                </td>
                <td style={tdStyle}>
                  <span style={badgeStyle(res.status)}>{res.status}</span>
                </td>
                <td style={tdStyle}>
                  <div style={{ display: 'flex', gap: '8px' }}>
                    
                    {showAdminActions && (
                      <>
                        <button
                          onClick={() => handleUpdateStatus(res.id, 'Approved')}
                          disabled={isFinalState}
                          style={{
                            padding: '6px 12px',
                            backgroundColor: isFinalState ? '#cbd5e1' : '#3b82f6',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            fontSize: '13px',
                            fontWeight: '600',
                            cursor: isFinalState ? 'not-allowed' : 'pointer'
                          }}
                        >
                          Authorize
                        </button>
                        <button
                          onClick={() => handleUpdateStatus(res.id, 'Rejected')}
                          disabled={isFinalState}
                          style={{
                            padding: '6px 12px',
                            backgroundColor: isFinalState ? '#cbd5e1' : '#ef4444',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            fontSize: '13px',
                            fontWeight: '600',
                            cursor: isFinalState ? 'not-allowed' : 'pointer'
                          }}
                        >
                          Deny
                        </button>
                      </>
                    )}

                    {!showAdminActions && isPending && (
                      <button
                        onClick={() => handleCancelBooking(res.id)}
                        style={{
                          padding: '6px 12px',
                          backgroundColor: '#b91c1c',
                          color: 'white',
                          border: 'none',
                          borderRadius: '4px',
                          fontSize: '13px',
                          fontWeight: '600',
                          cursor: 'pointer'
                        }}
                      >
                        Cancel
                      </button>
                    )}

                    {isFinalState && !isPending && (
                      <span style={{ fontSize: '12px', color: '#94a3b8', fontStyle: 'italic', alignSelf: 'center' }}>
                        Processed
                      </span>
                    )}

                  </div>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

export default ReservationsTable;