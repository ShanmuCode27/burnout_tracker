import './App.css'
import { BrowserRouter, Route, Routes } from 'react-router-dom'

import Dashboard from './pages/Dashboard'
import Login from './pages/Login';
import Signup from './pages/Signup'
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import PageLayout from './components/layouts/PageLayout';
import RepoDevelopersPage from './pages/RepoDevelopers';
import DeveloperDetailPage from './pages/DeveloperDetailPage';

function App() {

  return (
    <>
      <AuthProvider>
        <BrowserRouter>
          <PageLayout>
            <Routes>
              <Route
                path="/"
                element={
                  <ProtectedRoute>
                    <Dashboard />
                  </ProtectedRoute>
                }
              />
              <Route 
                path="/repos/:repoId" 
                element={
                  <ProtectedRoute>
                    <RepoDevelopersPage />
                  </ProtectedRoute>

                } 
              />
              <Route 
                path="/repos/:repoId/devs/:login" 
                element={
                  <ProtectedRoute>
                    <DeveloperDetailPage />
                  </ProtectedRoute>
                } 
              />
            <Route path="/login" element={<Login />} />
            <Route path="/signup" element={<Signup />} />
          </Routes>
        </PageLayout>
      </BrowserRouter>
    </AuthProvider >
    </>
  )
}

export default App
