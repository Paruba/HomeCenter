import React, { createContext, useContext, useState } from 'react';
import Cookies from 'js-cookie';

const AuthContext = createContext(null);

export const useAuth = () => {
    return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {

    const [user, setUser] = useState(() => {
        const storedUser = localStorage.getItem('user');
        return storedUser ? JSON.parse(storedUser) : null;
    });

    const [isLoggedIn, setIsLoggedIn] = useState(Cookies.get('AccessToken') ? true : false);

    const handleUserChange = (userDetail) => {
        localStorage.setItem('user', JSON.stringify(userDetail));
        setUser(userDetail);
        setIsLoggedIn(true);
    }

    const logout = () => {
        Cookies.remove('AccessToken');
        setUser(null);
        localStorage.removeItem('user');
        setIsLoggedIn(false);
    };

    const isInRole = (roleName) => {
        if (user) {
            const hasRole = user.roles.some(item => item.name === roleName);
            return hasRole;
        }
        return false;
    }

    return (
        <AuthContext.Provider value={{ isLoggedIn, logout, handleUserChange, isInRole }}>
            {children}
        </AuthContext.Provider>
    );
};
