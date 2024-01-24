import React, { useState, useContext } from 'react';
import AuthService from './Service/AuthService';
import { useAuth } from './AuthContext';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import AlertBox from '../Shared/AlertBox';
import { useNavigate } from 'react-router-dom';

const LoginComponent = () => {
    const [credentials, setCredentials] = useState({ username: '', password: '' });
    const { handleUserChange } = useAuth();
    const [errorMessage, setErrorMessage] = useState(0);
    const navigate = useNavigate();

    const handleLogin = async () => {
        try {
            const response = await AuthService.login(credentials);
            handleUserChange(response.user);
            console.log(response.user);
            setErrorMessage(null);
            navigate('/');
        } catch (error) {
            console.error(error);
            setErrorMessage("Invalid credentials.");
        }
    };

    return (
        <Box sx={{ width: '100%', maxWidth: 800, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2, marginTop: 5 }} className="TemperatureTable">
            <TextField
                label="Username"
                variant="outlined"
                value={credentials.username}
                onChange={(e) => setCredentials({ ...credentials, username: e.target.value })}
            />
            <TextField
                label="Heslo"
                type="password"
                variant="outlined"
                value={credentials.password}
                onChange={(e) => setCredentials({ ...credentials, password: e.target.value })}
            />
            <Button variant="contained" color="error" onClick={handleLogin}>
                Login
            </Button>
            <AlertBox errorMessage={errorMessage} />
        </Box>
    );
};

export default LoginComponent;