import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import React, { useState, useEffect } from 'react';
import TextField from '@mui/material/TextField';
import ThermometerService from './Service/ThermometerService';
import { useNavigate } from 'react-router-dom';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';

const ThermometerCreate = () => {
    GlobalKeyHandler();
    const navigate = useNavigate();
    const [termometerModel, setTermometerModel] = useState({
        name: ''
    });
    const [errors, setErrors] = useState({ name: '' });

    const handleCreate = async () => {
        const { isValid, validationErrors } = ThermometerService.termometerValidation(termometerModel);
        setErrors(validationErrors);
        if (isValid) {
            try {
                const response = await ThermometerService.create(termometerModel);
                if (!response.ok) {
                    setError("Požadavek vytvoření teploměru se nezdařil.")
                }
                navigate('/thermometers');
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
                    variant="standard"
                    value={termometerModel.name}
                    onChange={(e) => setTermometerModel({ ...termometerModel, name: e.target.value })}
                    error={!!errors.name}
                />
            </div>
            <div>
                <Button variant="contained" color="error" onClick={handleCreate}>
                    Potvrdit
                </Button>
            </div>
        </Box>
    );
};

export default ThermometerCreate;