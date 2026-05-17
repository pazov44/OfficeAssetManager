function Badge({ status }) {
  const isApproved = status === 'Approved';
  const isRejected = status === 'Rejected';
  
  const style = {
    padding: '4px 8px',
    borderRadius: '4px',
    fontSize: '12px',
    fontWeight: '600',
    backgroundColor: isApproved ? '#d1fae5' : isRejected ? '#fee2e2' : '#fef3c7',
    color: isApproved ? '#065f46' : isRejected ? '#991b1b' : '#92400e'
  };

  return <span style={style}>{status}</span>;
}

export default Badge;