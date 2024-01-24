import React, { useState, useEffect, useContext } from 'react';
import { Box, Select, MenuItem } from '@mui/material';
import AuthService from './Service/AuthService';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { Button, FormControl, InputLabel } from '@mui/material';

const UserDetail = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const userId = searchParams.get('userId');
    const [roles, setRoles] = useState([]);
    const [userModel, setUserModel] = useState({
        id: '',
        username: '',
        roles: []
    });
    const [selectedRoles, setSelectedRoles] = useState([]);

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

    const fetchUser = async (userId) => {
        try {
            await fetchRoles();
            const response = await AuthService.getUser(userId);
            if (response) {
                setUserModel(response);
                setSelectedRoles(response.roles);
            }
        } catch (error) {
            console.error(error)
        }
    };

    const handleRoleChange = (event) => {
        const {
            target: { value },
        } = event;

        const selectedRoleObjects = roles.filter(role => value.includes(role.id));
        setSelectedRoles(selectedRoleObjects);

        const roleIds = selectedRoleObjects.map(role => role.id);
        setUserModel({ ...userModel, roles: roleIds });
    };

    const handleUserEdit = async () => {
        try {
            const response = await AuthService.editUser(userModel);
            if (response.ok) {
                navigate('/users');
            }
                
        } catch (error) {
            console.error(error);
        }
    }

    useEffect(() => {
        fetchUser(userId);
    }, []);

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
            {roles && roles.length > 0 && (
                <div>
                    <FormControl sx={{ m: 1, width: '25ch' }} variant="standard">
                        <InputLabel id="demo-multiple-name-label">Role</InputLabel>
                        <Select
                            labelId="demo-multiple-name-label"
                            id="demo-multiple-name"
                            multiple
                            value={selectedRoles.map(role => role.id)}
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
            <Button variant="contained" color="error" onClick={handleUserEdit}>
                Potvrdit
            </Button>
        </Box>
    );
};

export default UserDetail;
