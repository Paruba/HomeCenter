import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Box from '@mui/material/Box';
import IconButton from '@mui/material/IconButton';
import Input from '@mui/material/Input';
import Button from '@mui/material/Button';
import InputLabel from '@mui/material/InputLabel';
import InputAdornment from '@mui/material/InputAdornment';
import FormControl from '@mui/material/FormControl';
import TextField from '@mui/material/TextField';
import FormHelperText from '@mui/material/FormHelperText';
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import CircularProgress from '@mui/material/CircularProgress';
import Typography from '@mui/material/Typography';
import UserService from './Service/UserService';

const UserPasswordChange = () => {
    const { userId } = useParams();
    const navigate = useNavigate();
    const [registrationModel, setRegistrationModel] = useState({
        username: '',
        password: '',
        repeatedPassword: '',
        oldPassword: ''
    });
    const [loading, setLoading] = useState(true);
    const [showPassword, setShowPassword] = useState(false);
    const [apiError, setError] = useState(null);
    const [errors, setErrors] = useState({ username: '', password: '', repeatedPassword: '' });

    const handleClickShowPassword = () => setShowPassword((show) => !show);
    const handleMouseDownPassword = (event) => event.preventDefault();

    useEffect(() => {
        const fetchUser = async () => {
            try {
                setLoading(true);
                const response = await UserService.getUser(userId);
                setRegistrationModel(response);
            } catch (err) {
                setError(err);
            } finally {
                setLoading(false);
            }
        };

        fetchUser();
    }, [userId]);

    const handlePasswordChange = async () => {
        const { isValid, validationErrors } = UserService.registrationValidation(registrationModel);
        setErrors(validationErrors);
        if (isValid) {
            try {
                const response = await UserService.changePassword(registrationModel, userId);
                if (!response.ok) {
                    setError("Požadavek zmìny hesla se nepovedl.")
                }
                navigate('/users');
            } catch (error) {
                console.error(error);
            }
        }
    };

    if (loading) {
        return <CircularProgress />;
    }

    if (apiError) {
        return <Typography>Error: {apiError.message}</Typography>;
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
                    label="Email"
                    variant="standard"
                    readOnly={true}
                    value={registrationModel.username}
                    error={!!errors.username}
                    helperText={errors.username}
                />
            </div>
            <div>
                <FormControl sx={{ m: 1, width: '25ch' }} variant="standard">
                    <InputLabel htmlFor="standard-adornment-password">Heslo</InputLabel>
                    <Input
                        name="password"
                        type={showPassword ? 'text' : 'password'}
                        onChange={(e) => setRegistrationModel({ ...registrationModel, password: e.target.value })}
                        error={!!errors.password}
                        endAdornment={
                            <InputAdornment position="end">
                                <IconButton
                                    aria-label="toggle password visibility"
                                    onClick={handleClickShowPassword}
                                    onMouseDown={handleMouseDownPassword}
                                >
                                    {showPassword ? <VisibilityOff /> : <Visibility />}
                                </IconButton>
                            </InputAdornment>
                        }
                    />
                    <FormHelperText>{errors.password}</FormHelperText>
                </FormControl>
            </div>
            <div>
                <FormControl sx={{ m: 1, width: '25ch' }} variant="standard">
                    <InputLabel htmlFor="standard-adornment-repeat-password">Kontrola hesla</InputLabel>
                    <Input
                        name="repeatedPassword"
                        type={showPassword ? 'text' : 'password'}
                        onChange={(e) => setRegistrationModel({ ...registrationModel, repeatedPassword: e.target.value })}
                        error={!!errors.repeatedPassword}
                        endAdornment={
                            <InputAdornment position="end">
                                <IconButton
                                    aria-label="toggle password visibility"
                                    onClick={handleClickShowPassword}
                                    onMouseDown={handleMouseDownPassword}
                                >
                                    {showPassword ? <VisibilityOff /> : <Visibility />}
                                </IconButton>
                            </InputAdornment>
                        }
                    />
                    <FormHelperText>{errors.repeatedPassword}</FormHelperText>
                </FormControl>
            </div>
            <div>
                <Button variant="contained" color="error" onClick={handlePasswordChange}>
                    Potvrdit
                </Button>
            </div>
        </Box>
    );
};

export default UserPasswordChange;
