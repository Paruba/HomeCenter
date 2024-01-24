import Alert from '@mui/material/Alert';

const AlertBox = ({ errorMessage }) => {
    if (errorMessage) {
        return (
            <div className="mt-10">
                <Alert className="TemperatureAlertBox" severity="error">
                    {(errorMessage)}
                </Alert>
            </div>
        );
    }
    return null;
};

export default AlertBox;
