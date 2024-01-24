import React, { useState, useContext, useEffect } from 'react';
import { Box, Select, MenuItem } from '@mui/material';
import IconButton from '@mui/material/IconButton';
import Input from '@mui/material/Input';
import Button from '@mui/material/Button';
import InputLabel from '@mui/material/InputLabel';
import InputAdornment from '@mui/material/InputAdornment';
import AuthService from './Service/AuthService';
import FormControl from '@mui/material/FormControl';
import TextField from '@mui/material/TextField';
import FormHelperText from '@mui/material/FormHelperText';
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import AlertBox from '../Shared/AlertBox';
import { useNavigate } from 'react-router-dom';
import UserService from './Service/UserService';

const RegistrationForm = () => {
    const navigate = useNavigate();
    const [showPassword, setShowPassword] = React.useState(false);
    const [roles, setRoles] = useState(null);
    const [selectedRoles, setSelectedRoles] = useState([]);
    const [registrationModel, setRegistrationModel] = useState({
        username: '',
        password: '',
        repeatedPassword: '',
        roles: []
    });
    const [errors, setErrors] = useState({ username: '', password: '', repeatedPassword: '' });
    const [errorMessage, setErrorMessage] = useState(0);
    const handleClickShowPassword = () => setShowPassword((show) => !show);

    useEffect(() => {
        fetchRoles();
    }, []);

    const fetchRoles = async () => {
        try {
            const response = await AuthService.getRoles();
            if (response) {
                setRoles(response);
            }
        } catch (error) {
            console.error(error);
        }
    };

    const handleMouseDownPassword = (event) => {
        event.preventDefault();
    };

    const handleRoleChange = (event) => {
        const {
            target: { value },
        } = event;

        const selectedRoleObjects = roles.filter(role => value.includes(role.id));
        setSelectedRoles(selectedRoleObjects);

        const roleIds = selectedRoleObjects.map(role => role.id);
        setRegistrationModel({ ...registrationModel, roles: roleIds });
    };
    
    const handleRegistration = async () => {
        const { isValid, validationErrors } = UserService.registrationValidation(registrationModel)
        setErrors(validationErrors);
        if (isValid) {
            try {
                const response = await AuthService.registration(registrationModel);
                if (!response.ok) {
                    setErrorMessage('Požadavek k vytvoøení uživatele se nepovedl.')
                }
                navigate('/users');
            } catch (error) {
                console.error(error);
            }
        }
    };

    return (
        <Box
            component="form"
            sx={{
                '& .MuiTextField-root': { m: 1, width: '25ch' },
            }}
            noValidate
            autoComplete="off"
            className="TemperatureBox"
        >
            <div>
                <TextField label="Email"
                    variant="standard"
                    value={registrationModel.username}
                    onChange={(e) => setRegistrationModel({ ...registrationModel, username: e.target.value })}
                    error={!!errors.username}
                    helperText={errors.username} />
            </div>
            <div>
                <FormControl sx={{ m: 1, width: '25ch' }} variant="standard">
                    <InputLabel htmlFor="standard-adornment-password">Heslo</InputLabel>
                    <Input
                        name="password"
                        type={showPassword ? 'text' : 'password'}
                        value={registrationModel.password}
                        onChange={(e) => setRegistrationModel({ ...registrationModel, password: e.target.value })}
                        error={!!errors.username}
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
                        value={registrationModel.repeatedPassword}
                        onChange={(e) => setRegistrationModel({ ...registrationModel, repeatedPassword: e.target.value })}
                        error={!!errors.username}
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
                {roles && roles.length > 0 && (
                    <div>
                        <FormControl sx={{ m: 1, width: '25ch' }} variant="standard">
                            <InputLabel id="demo-multiple-name-label">Role</InputLabel>
                            <Select
                                labelId="demo-multiple-name-label"
                                id="demo-multiple-name"
                                multiple
                                value={selectedRoles.map(role => role.id)} // Use role IDs for value
                                onChange={handleRoleChange}
                                renderValue={(selected) => selected.map(roleId => roles.find(role => role.id === roleId).name).join(', ')} // Display role names
                            >
                                {roles.map((role, index) => (
                                    <MenuItem key={index} value={role.id}>
                                        {role.name}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </div>
                )}
            </div>
            <div>
                <Button variant="contained" color="error" onClick={handleRegistration}>
                    Registrace
                </Button>
                <AlertBox errorMessage={errorMessage} />
            </div>
        </Box>
    );
};

export default RegistrationForm;