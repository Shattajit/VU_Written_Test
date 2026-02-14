import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalUsers, setTotalUsers] = useState(0);
  
  // Form state
  const [formData, setFormData] = useState({
    name: '',
    age: '',
    email: ''
  });
  const [formSubmitting, setFormSubmitting] = useState(false);
  const [formSuccess, setFormSuccess] = useState(false);

  const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;
  console.log("ENV:", process.env.REACT_APP_API_BASE_URL);

  // Fetch users with pagination
  const fetchUsers = async (page = 1) => {
    setLoading(true);
    setError(null);
    try {
      const response = await axios.get(`${API_BASE_URL}/fetch-users`, {
        params: {
          page: page,
          pageSize: 100
        }
      });
      
      console.log('API Response:', response.data); 
      
      setUsers(response.data.Users || response.data.users || []);
      setCurrentPage(response.data.Page || response.data.page || 1);
      setTotalPages(response.data.TotalPages || response.data.totalPages || 1);
      setTotalUsers(response.data.TotalUsers || response.data.totalUsers || 0);
    } catch (err) {
      setError('Failed to fetch users: ' + err.message);
      console.error('Error fetching users:', err);
    } finally {
      setLoading(false);
    }
  };

  // Create bulk users
  const createBulkUsers = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await axios.post(`${API_BASE_URL}/create-bulk-users`);
      alert(`Success! Created ${response.data.count || response.data.Count} users in ${(response.data.durationSeconds || response.data.DurationSeconds).toFixed(2)} seconds`);
      fetchUsers(1); 
    } catch (err) {
      setError('Failed to create bulk users: ' + err.message);
      console.error('Error creating bulk users:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setFormSubmitting(true);
    setFormSuccess(false);
    setError(null);

    try {
      const response = await axios.post(`${API_BASE_URL}/create-users`, formData);
      setFormSuccess(true);
      setFormData({ name: '', age: '', email: '' }); 
      
      alert(`User created successfully! ID: ${response.data.id || response.data.Id}`);
      
      fetchUsers(currentPage);
    } catch (err) {
      setError('Failed to create user: ' + err.message);
      console.error('Error creating user:', err);
    } finally {
      setFormSubmitting(false);
    }
  };

  useEffect(() => {
    fetchUsers(1);
  }, []);

  const goToPage = (page) => {
    if (page >= 1 && page <= totalPages) {
      fetchUsers(page);
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>User Management System</h1>
        <p>Total Users: {totalUsers}</p>
      </header>

      <div className="container">
        {/* Create User Form */}
        <div className="form-section">
          <h2>Create New User</h2>
          <form onSubmit={handleSubmit} className="user-form">
            <div className="form-group">
              <label htmlFor="name">Name:</label>
              <input
                type="text"
                id="name"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                required
                placeholder="Enter full name"
              />
            </div>

            <div className="form-group">
              <label htmlFor="age">Age:</label>
              <input
                type="number"
                id="age"
                name="age"
                value={formData.age}
                onChange={handleInputChange}
                required
                min="18"
                max="120"
                placeholder="Enter age"
              />
            </div>

            <div className="form-group">
              <label htmlFor="email">Email:</label>
              <input
                type="email"
                id="email"
                name="email"
                value={formData.email}
                onChange={handleInputChange}
                required
                placeholder="Enter email address"
              />
            </div>

            <button 
              type="submit" 
              className="btn btn-primary"
              disabled={formSubmitting}
            >
              {formSubmitting ? 'Creating...' : 'Create User'}
            </button>
          </form>

          {formSuccess && (
            <div className="success-message">
              ✓ User created successfully!
            </div>
          )}
        </div>

        {/* Action Buttons */}
        <div className="actions">
          <button 
            onClick={() => fetchUsers(currentPage)} 
            className="btn btn-secondary"
            disabled={loading}
          >
            {loading ? 'Loading...' : 'Refresh Users'}
          </button>
          <button 
            onClick={createBulkUsers} 
            className="btn btn-warning"
            disabled={loading}
          >
            Create 10,000 Users
          </button>
        </div>

        {/* Error Message */}
        {error && <div className="error-message">{error}</div>}

        {/* Users Table */}
        <div className="table-section">
          <h2>User List (Page {currentPage} of {totalPages})</h2>
          
          {loading ? (
            <div className="loading">Loading users...</div>
          ) : (
            <>
              <table className="user-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Age</th>
                    <th>Email</th>
                    <th>Timestamp</th>
                  </tr>
                </thead>
                <tbody>
                  {users.length === 0 ? (
                    <tr>
                      <td colSpan="5" className="no-data">
                        No users found. Click "Create 10,000 Users" to add data.
                      </td>
                    </tr>
                  ) : (
                    users.map(user => (
                      <tr key={user.Id || user.id}>
                        <td>{user.Id || user.id}</td>
                        <td>{user.Name || user.name}</td>
                        <td>{user.Age || user.age}</td>
                        <td>{user.Email || user.email}</td>
                        <td>{new Date(user.TimeStamp || user.timeStamp).toLocaleString()}</td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="pagination">
                  <button 
                    onClick={() => goToPage(1)} 
                    disabled={currentPage === 1}
                    className="btn btn-small"
                  >
                    First
                  </button>
                  <button 
                    onClick={() => goToPage(currentPage - 1)} 
                    disabled={currentPage === 1}
                    className="btn btn-small"
                  >
                    Previous
                  </button>
                  
                  <span className="page-info">
                    Page {currentPage} of {totalPages}
                  </span>
                  
                  <button 
                    onClick={() => goToPage(currentPage + 1)} 
                    disabled={currentPage === totalPages}
                    className="btn btn-small"
                  >
                    Next
                  </button>
                  <button 
                    onClick={() => goToPage(totalPages)} 
                    disabled={currentPage === totalPages}
                    className="btn btn-small"
                  >
                    Last
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}

export default App;