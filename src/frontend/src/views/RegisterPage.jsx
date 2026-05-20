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
  
  // Focus states for contextual rendering
  const [isPasswordFocused, setIsPasswordFocused] = useState(false);
  const [isUsernameFocused, setIsUsernameFocused] = useState(false);

  // Real-time username validation (only alphanumeric)
  const isUsernameValid = userName.length > 0 && /^[a-zA-Z0-9]+$/.test(userName);

  // Real-time password validation tracking state
  const passwordRules = {
    minLength: password.length >= 8,
    hasLower: /[a-z]/.test(password),
    hasUpper: /[A-Z]/.test(password),
    hasNumber: /[0-9]/.test(password),
    hasSymbol: /[^A-Za-z0-9]/.test(password),
  };
  const isPasswordValid = Object.values(passwordRules).every(Boolean);

  // Global form validity
  const isFormValid = isUsernameValid && isPasswordValid;

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    if (!isFormValid) {
      setError('Please satisfy all username and password requirements.');
      return;
    }

    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    try {
      await authService.register(email, password, userName);
      setSuccess(true);
      alert('Registration successful! Redirecting to login...');
      
      setTimeout(() => {
        window.location.href = '/';
      }, 1500);
    } catch (err) {
      setError(err.response?.data?.message || 'Registration failed. Try a different username/email.');
    }
  };

  const getRuleStyle = (isValid) => ({
    color: isValid ? '#28a745' : '#dc3545',
    fontSize: '13px',
    margin: '4px 0',
    display: 'flex',
    alignItems: 'center',
    transition: 'color 0.2s ease'
  });

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', backgroundColor: '#f5f5f5', fontFamily: 'sans-serif' }}>
      <form onSubmit={handleSubmit} style={{ background: 'white', padding: '30px', borderRadius: '8px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', width: '320px' }}>
        <h3 style={{ margin: '0 0 20px 0', textAlign: 'center', color: '#666', fontSize: '18px' }}>Sign Up</h3>
        
        {error && <p style={{ color: 'red', fontSize: '14px', backgroundColor: '#ffeef0', padding: '8px', borderRadius: '4px', border: '1px solid #fecdd3' }}>{error}</p>}
        {success && <p style={{ color: 'green', fontSize: '14px', backgroundColor: '#f0fdf4', padding: '8px', borderRadius: '4px', border: '1px solid #bbf7d0' }}>Account created successfully!</p>}

        {/* Username Field */}
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: '500' }}>Username</label>
          <input 
            type="text" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ccc' }}
            value={userName}
            onChange={(e) => setUserName(e.target.value)} 
            onFocus={() => setIsUsernameFocused(true)}
            onBlur={() => setIsUsernameFocused(false)}
            required
          />
          
          {/* Real-time Username Indicator */}
          {isUsernameFocused && (
            <div style={{ marginTop: '10px', padding: '8px', backgroundColor: '#fafafa', borderRadius: '4px', border: '1px solid #eee' }}>
              <ul style={{ listStyleType: 'none', paddingLeft: '5px', margin: '0' }}>
                <li style={getRuleStyle(isUsernameValid)}>
                  {isUsernameValid ? '✓' : '✗'} Letters and numbers only (no spaces/symbols)
                </li>
              </ul>
            </div>
          )}
        </div>

        {/* Email Field */}
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: '500' }}>Email Address</label>
          <input 
            type="email" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ccc' }}
            value={email}
            onChange={(e) => setEmail(e.target.value)} 
            required
          />
        </div>

        {/* Password Field */}
        <div style={{ marginBottom: '15px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: '500' }}>Password</label>
          <input 
            type="password" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ccc' }}
            value={password}
            onChange={(e) => setPassword(e.target.value)} 
            onFocus={() => setIsPasswordFocused(true)}
            onBlur={() => setIsPasswordFocused(false)}
            required
          />
          
          {/* Real-time Password Indicator */}
          {isPasswordFocused && (
            <div style={{ marginTop: '10px', padding: '8px', backgroundColor: '#fafafa', borderRadius: '4px', border: '1px solid #eee' }}>
              <p style={{ margin: '0 0 6px 0', fontSize: '12px', fontWeight: 'bold', color: '#555' }}>Password must include:</p>
              <ul style={{ listStyleType: 'none', paddingLeft: '5px', margin: '0' }}>
                <li style={getRuleStyle(passwordRules.minLength)}>
                  {passwordRules.minLength ? '✓' : '✗'} Minimum 8 characters
                </li>
                <li style={getRuleStyle(passwordRules.hasLower)}>
                  {passwordRules.hasLower ? '✓' : '✗'} At least one lowercase letter
                </li>
                <li style={getRuleStyle(passwordRules.hasUpper)}>
                  {passwordRules.hasUpper ? '✓' : '✗'} At least one uppercase letter
                </li>
                <li style={getRuleStyle(passwordRules.hasNumber)}>
                  {passwordRules.hasNumber ? '✓' : '✗'} At least one number
                </li>
                <li style={getRuleStyle(passwordRules.hasSymbol)}>
                  {passwordRules.hasSymbol ? '✓' : '✗'} At least one special character
                </li>
              </ul>
            </div>
          )}
        </div>

        {/* Confirm Password Field */}
        <div style={{ marginBottom: '20px' }}>
          <label style={{ display: 'block', marginBottom: '5px', fontWeight: '500' }}>Confirm Password</label>
          <input 
            type="password" 
            style={{ width: '100%', padding: '8px', boxSizing: 'border-box', borderRadius: '4px', border: '1px solid #ccc' }}
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)} 
            required
          />
        </div>

        {/* Submit Button guarded by global form validity */}
        <button 
          type="submit" 
          disabled={!isFormValid}
          style={{ 
            width: '100%', 
            padding: '10px', 
            backgroundColor: isFormValid ? '#28a745' : '#6c757d', 
            color: 'white', 
            border: 'none', 
            borderRadius: '4px', 
            cursor: isFormValid ? 'pointer' : 'not-allowed', 
            fontWeight: 'bold', 
            marginBottom: '15px',
            transition: 'background-color 0.2s'
          }}
        >
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