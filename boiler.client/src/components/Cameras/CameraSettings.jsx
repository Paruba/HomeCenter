import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import CameraService from './Service/CameraService';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import { Button } from '@mui/base/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Typography from '@mui/material/Typography';
import { useNavigate } from 'react-router-dom';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';

const CameraSettings = () => {
    GlobalKeyHandler();

    const [errors, setErrors] = useState({ name: '' });
    const [cameraSettingModel, setCameraConfigModel] = useState({
        name: '',
        period: 30
    });
    const [searchParams] = useSearchParams();
    const cameraId = searchParams.get('cameraId');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isReadOnly, setIsReadOnly] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchCamera = async () => {
            try {
                setLoading(true);
                const response = await CameraService.getCameraConfig(cameraId);
                setCameraConfigModel(response);
            } catch (err) {
                setError(err);
            } finally {
                setLoading(false);
            }
        };

        fetchCamera();
    }, []);

    const handleConfig = async () => {
        const { isValid, validationErrors } = CameraService.cameraValidation(cameraSettingModel);
        setErrors(validationErrors);
        if (isValid) {
            try {
                const response = await CameraService.update(cameraSettingModel);
                navigate('/cameras');
            } catch (error) {
                console.error(error);
            }
        }
    };

    const handleUnlock = () => {
        setIsReadOnly(!isReadOnly);
    };

    if (loading) {
        return <CircularProgress />;
    }

    if (error) {
        return <Typography>Error: {error.message}</Typography>;
    }

    return (
        <Box
            component="form"
            sx={{ '& .MuiTextField-root': { m: 1, width: '25ch' } }}
            noValidate
            autoComplete="off"
            className="TemperatureBox"
        >
            <div>
                <TextField
                    label={"N\u00E1zev"}
                    variant="outlined"
                    value={cameraSettingModel.name}
                    onChange={(e) => setCameraConfigModel({ ...cameraSettingModel, name: e.target.value })}
                    error={!!errors.name}
                    disabled={isReadOnly}
                />
                <TextField
                    label="Perioda"
                    type="number"
                    value={cameraSettingModel.period}
                    onChange={(e) => setCameraConfigModel({ ...cameraSettingModel, period: e.target.value })}
                    InputLabelProps={{
                        shrink: true,
                    }}
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    inputProps={{
                        min: 1,
                        step: 1
                    }}
                    disabled={isReadOnly}
                />
            </div>
            <div style={{ float: 'right' }}>
                {isReadOnly ? <Button variant="contained" color="error" onClick={handleUnlock}>Odemknout</Button> : <Button variant="contained" color="error" onClick={handleUnlock}>Zamknout</Button>}
                <Button variant="contained" color="error" onClick={handleConfig} style={{ marginLeft: '5px' }}>
                    Potvrdit
                </Button>
            </div>
        </Box>
    );
};

export default CameraSettings;
