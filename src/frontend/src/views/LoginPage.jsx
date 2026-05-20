import { useState } from 'react';
import { authService } from '../services/authService';
import { Link } from 'react-router-dom';

function LoginPage() {
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault(); // Stops the browser from doing a full page reload
    setError('');

    try {
      await authService.login(userName, password);
      alert('Login successful! Token saved.');
      window.location.href = '/dashboard'; // Redirect to dashboard
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid username or password');
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', backgroundColor: '#f5f5f5', fontFamily: 'sans-serif' }}>
      <form onSubmit={handleSubmit} style={{ background: 'white', padding: '30px', borderRadius: '8px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', width: '300px' }}>
        <h3>Sign In</h3>
        
        {error && <p style={{ color: 'red', fontSize: '14px' }}>{error}</p>}

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Username</label>
          <input 
            type="text" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
            value={userName}
            onChange={(e) => setUserName(e.target.value)} 
            required
          />
        </div>

        <div style={{ marginBottom: '20px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Password</label>
          <input 
            type="password" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
            value={password}
            onChange={(e) => setPassword(e.target.value)} 
            required
          />
        </div>

        <button type="submit" style={{ width: '100%', padding: '10px', backgroundColor: '#007bff', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}>
          Login
        </button>


<p style={{ textAlign: 'center', fontSize: '14px', marginTop: '15px', marginBottom: '0' }}>
  Don't have an account?{' '}
  <Link to="/register" style={{ color: '#007bff', textDecoration: 'none' }}>Sign Up</Link>
</p>

      </form>
    </div>
  );
}

export default LoginPage;