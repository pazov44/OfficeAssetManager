import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from "./views/LoginPage.jsx";
import DashboardPage from "./views/DashboardPage.jsx";
import RegisterPage from './views/RegisterPage';

function App() {
  const isAuthenticated = !!localStorage.getItem('token');

  return (
    <Router>
      <Routes>
        {/* Public Route */}
        <Route path="/" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Protected Route: If not authenticated, kick user back to login */}
        <Route 
          path="/dashboard" 
          element={isAuthenticated ? <DashboardPage /> : <Navigate to="/" />} 
        />
      </Routes>
    </Router>
  );
}

export default App;