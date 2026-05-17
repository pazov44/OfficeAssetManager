import { useState } from 'react';
import { authService } from '../services/authService';
import { Link } from 'react-router-dom';

function RegisterPage() {
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    // Front-end validation: Ensure passwords match before hitting the API
    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    try {
      // Calls your authService.register(email, password, userName)
      await authService.register(email, password, userName);
      setSuccess(true);
      alert('Registration successful! Redirecting to login...');
      
      // Redirect to login page after a successful registration
      setTimeout(() => {
        window.location.href = '/';
      }, 1500);
    } catch (err) {
      // Pulls out custom validation errors sent back from ASP.NET Identity
      setError(err.response?.data?.message || 'Registration failed. Try a different username/email.');
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', backgroundColor: '#f5f5f5', fontFamily: 'sans-serif' }}>
      <form onSubmit={handleSubmit} style={{ background: 'white', padding: '30px', borderRadius: '8px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', width: '300px' }}>
        <h2>Office Asset Manager</h2>
        <h3>Create Account</h3>
        
        {error && <p style={{ color: 'red', fontSize: '14px' }}>{error}</p>}
        {success && <p style={{ color: 'green', fontSize: '14px' }}>Account created successfully!</p>}

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

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Email Address</label>
          <input 
            type="email" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
            value={email}
            onChange={(e) => setEmail(e.target.value)} 
            required
          />
        </div>

        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Password</label>
          <input 
            type="password" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
            value={password}
            onChange={(e) => setPassword(e.target.value)} 
            required
          />
        </div>

        <div style={{ marginBottom: '20px' }}>
          <label style={{ display: 'block', marginBottom: '5px' }}>Confirm Password</label>
          <input 
            type="password" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box' }}
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)} 
            required
          />
        </div>

        <button type="submit" style={{ width: '100%', padding: '10px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold', marginBottom: '15px' }}>
          Sign Up
        </button>

<p style={{ textAlign: 'center', fontSize: '14px', margin: '0' }}>
  Already have an account?{' '}
  <Link to="/" style={{ color: '#007bff', textDecoration: 'none' }}>Sign In</Link>
</p>
      </form>
    </div>
  );
}

export default RegisterPage;