import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import TemperatureDisplay from './components/TemperatureDisplay';
import NavigationAppBar from './components/Shared/NavigationAppBar';
import './App.css';
import LoginComponent from './components/Identity/LoginForm';
import { AuthProvider } from './components/Identity/AuthContext'; 
import RegistrationForm from './components/Identity/RegistrationForm';
import UsersList from './components/Identity/UsersList';
import UserPasswordChange from './components/Identity/UserPasswordChange';
import ThermometerList from './components/Thermometers/ThermometerList';
import ThermometerCreate from './components/Thermometers/ThermometerCreate';
import CameraList from './components/Cameras/CameraList';
import CameraCreate from './components/Cameras/CameraCreate';
import CameraDetail from './components/Cameras/CameraDetail';
import VideoArchive from './components/Cameras/VideoArchive';
import ImageStream from './components/Cameras/ImageStream'; 
import CameraSettings from './components/Cameras/CameraSettings';
import TokenGenerator from './components/Cameras/TokenGenerator';
import UserDetail from './components/Identity/UserDetail';
import RecognizedFaces from './components/Cameras/RecognizedFaces';
import ProtectedRoute from './framework/routes/ProtectedRoute';
import AdminRoute from './framework/routes/AdminRoute';

function App() {
    return (
        <div>
            <AuthProvider>
                <Router>
                    <NavigationAppBar />
                    <Routes>
                        <Route path="/change-password/:userId" element={<ProtectedRoute><UserPasswordChange /></ProtectedRoute>} />
                        <Route path="/users" element={<AdminRoute><UsersList /></AdminRoute>} />
                        <Route path="/user-detail" element={<AdminRoute><UserDetail /></AdminRoute>} />
                        <Route path="/registration" element={<AdminRoute><RegistrationForm /></AdminRoute>} />
                        <Route path="/login" element={<LoginComponent />} />
                        <Route path="/thermometers" element={<ProtectedRoute><ThermometerList /></ProtectedRoute>} />
                        <Route path="/thermometer-create" element={<ProtectedRoute><ThermometerCreate /></ProtectedRoute>} />
                        <Route path="/temperature" element={<ProtectedRoute><TemperatureDisplay /></ProtectedRoute>} />
                        <Route path="/cameras" element={<ProtectedRoute><CameraList /></ProtectedRoute>} />
                        <Route path="/camera-create" element={<ProtectedRoute><CameraCreate /></ProtectedRoute>} />
                        <Route path="/camera-detail" element={<ProtectedRoute><CameraDetail /></ProtectedRoute>} />
                        <Route path="/video-archive" element={<ProtectedRoute><VideoArchive /></ProtectedRoute>} />
                        <Route path="/image-stream" element={<ProtectedRoute><ImageStream /></ProtectedRoute>} />
                        <Route path="/camera-setting" element={<ProtectedRoute><CameraSettings /></ProtectedRoute>} />
                        <Route path="/device-token" element={<ProtectedRoute><TokenGenerator /></ProtectedRoute>} />
                        <Route path="/detected-faces" element={<ProtectedRoute><RecognizedFaces /></ProtectedRoute>} />
                        <Route path="/" element={<ThermometerList />} />
                    </Routes>
                </Router>
            </AuthProvider>
        </div>
    );
}

export default App;