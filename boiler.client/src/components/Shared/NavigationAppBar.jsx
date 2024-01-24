import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import MenuIcon from '@mui/icons-material/Menu';
import IconButton from '@mui/material/IconButton';
import { useAuth } from '../Identity/AuthContext';

const NavigationAppBar = () => {
    const navigate = useNavigate();
    const { isLoggedIn, logout, isInRole } = useAuth();

    const [anchorEl, setAnchorEl] = React.useState(null);
    const open = Boolean(anchorEl);

    const handleClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = (path = null) => {
        setAnchorEl(null);
        if (path) {
            navigate(path);
        }
    };

    const handleLogin = () => {
        navigate('/login');
    };

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <AppBar position="fixed" style={{ width: '100%' }} className="TemperatureAppBar">
            <Toolbar>
                {isLoggedIn && (
                <>
                    <IconButton
                        id="basic-button"
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-controls={open ? 'basic-menu' : undefined}
                        aria-haspopup="true"
                        aria-expanded={open ? 'true' : undefined}
                        onClick={handleClick}
                        sx={{ mr: 2 }} >
                        <MenuIcon />
                    </IconButton>
                    <Menu
                        id="basic-menu"
                        anchorEl={anchorEl}
                        open={open}
                        onClose={handleClose}
                        MenuListProps={{
                            'aria-labelledby': 'basic-button',
                        }}
                        >
                            <MenuItem onClick={() => handleClose('/thermometers')}>Teploměry</MenuItem>
                            <MenuItem onClick={() => handleClose('/cameras')}>Kamery</MenuItem>
                            {isInRole('Admin') && (
                                <div>
                                    <MenuItem onClick={() => handleClose('/users')}>Uživatele</MenuItem>
                                    <MenuItem onClick={() => handleClose('/registration')}>Registrace</MenuItem>
                                </div>
                        )}
                    
                    </Menu>
                </>
                )}
                <Typography variant="h6" style={{ flexGrow: 1 }}>
                    Home center
                </Typography>
                {isLoggedIn ? (
                    <Button variant="contained" color="inherit" onClick={handleLogout}>Logout</Button>
                ) : (
                    <Button variant="contained" color="inherit" onClick={handleLogin}>Login</Button>
                )}
            </Toolbar>
        </AppBar>
    );
};

export default NavigationAppBar;
