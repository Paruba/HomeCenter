import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../components/Identity/AuthContext';

const AdminRoute = ({ children }) => {
    const { isLoggedIn, isInRole } = useAuth();
    const location = useLocation();

    return (isLoggedIn && isInRole('admin')) ? children : <Navigate to="/login" state={{ from: location }} replace />;
};

export default AdminRoute;