import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import React, { useState, useEffect } from 'react';
import TextField from '@mui/material/TextField';
import CameraService from './Service/CameraService';
import { useNavigate } from 'react-router-dom';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';

const CameraCreate = () => {
    GlobalKeyHandler();
    const navigate = useNavigate();
    const [cameraModel, setCameraModel] = useState({
        name: '',
        perioda: 30
    });
    const [errors, setErrors] = useState({ name: '' });

    const handleCreate = async () => {
        const { isValid, validationErrors } = CameraService.cameraValidation(cameraModel);
        setErrors(validationErrors);
        if (isValid) {
            try {
                const response = await CameraService.create(cameraModel);
                if (!response.ok) {
                    setError("Požadavek vytvoření kamery se nezdařil.")
                }
                navigate('/cameras');
            } catch (error) {
                console.error(error);
            }
        }
    };

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
                    value={cameraModel.name}
                    onChange={(e) => setCameraModel({ ...cameraModel, name: e.target.value })}
                    error={!!errors.name}
                />
                <TextField
                    label="Perioda"
                    type="number"
                    value={cameraModel.period}
                    onChange={(e) => setCameraConfigModel({ ...cameraModel, period: e.target.value })}
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
                />
            </div>
            <div style={{ float: 'right' }}>
                <Button variant="contained" color="error" onClick={handleCreate}>
                    Potvrdit
                </Button>
            </div>
        </Box>
    );
};

export default CameraCreate;