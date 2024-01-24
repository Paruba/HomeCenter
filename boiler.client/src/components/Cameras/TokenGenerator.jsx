import React, { useEffect, useState } from 'react';
import Typography from '@mui/material/Typography';
import { useSearchParams } from 'react-router-dom';
import CircularProgress from '@mui/material/CircularProgress';
import TextField from '@mui/material/TextField';
import AuthService from '../Identity/Service/AuthService';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';

const TokenGenerator = () => {
    GlobalKeyHandler();

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [token, setToken] = useState('');
    const [searchParams] = useSearchParams();
    const deviceId = searchParams.get('deviceId');
    const device = searchParams.get('device');

    const fetchToken = async () => {
        try {
            setLoading(true);
            const response = await AuthService.deviceToken(deviceId, device);
            setToken(response.token);
        } catch (err) {
            setError(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchToken();
    }, []);

    if (loading) {
        return <CircularProgress />;
    }

    if (error) {
        return <Typography>Error: {error.message}</Typography>;
    }

    return (
        <TextField
            id="outlined-multiline-flexible"
            label="Token"
            value={token}
            multiline
            maxRows={8}
            sx={{ width: '800px' }}
        />
    );
}

export default TokenGenerator;