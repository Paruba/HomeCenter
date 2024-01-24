import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const GlobalKeyHandler = () => {
    const navigate = useNavigate();

    useEffect(() => {
        const handleKeyDown = (event) => {
            if (event.key === 'Escape') {
                navigate(-1);
            }
        };

        window.addEventListener('keydown', handleKeyDown);

        return () => {
            window.removeEventListener('keydown', handleKeyDown);
        };
    }, [navigate]);
};

export default GlobalKeyHandler;