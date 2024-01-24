import AuthorizedApi from '../../../framework/http/AuthorizedApi';

const CameraService = {
    getCameras: async (searchValue, page, rowsPerPage) => {
        try {
            const take = rowsPerPage;
            const skip = page * take;
            const data = await AuthorizedApi.getJson(`camera?searchValue=${searchValue}&skip=${skip}&take=${take}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat kamery.", error);
            throw error;
        }
    },
    getCameraConfig: async (cameraId) => {
        try {
            const data = await AuthorizedApi.getJson(`camera/configuration?cameraId=${cameraId}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat konfiguraci kamery.", error);
            throw error;
        }
    },
    getVideos: async (cameraId, searchValue, page, rowsPerPage) => {
        try {
            const take = rowsPerPage;
            const skip = page * take;
            const data = await AuthorizedApi.getJson(`camera/videos?cameraId=${cameraId}&searchValue=${searchValue}&skip=${skip}&take=${take}`)
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat videa.", error);
            throw error;
        }
    },
    getVideo: async (cameraId, videoId) => {
        try {
            const data = await AuthorizedApi.getBlob(`camera/video?cameraId=${cameraId}&videoId=${videoId}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat video.", error);
            throw error;
        }
    },
    getLastVideo: async (cameraId) => {
        try {
            const data = await AuthorizedApi.getBlob(`camera/video-last?cameraId=${cameraId}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat video.", error);
            throw error;
        }
    },
    getVideoName: async (cameraId, videoId) => {
        try {
            const data = await AuthorizedApi.get(`camera/video-name?cameraId=${cameraId}&videoId=${videoId}`);
            return await data.text();
        } catch (error) {

        }
    },
    create: async (cameraModel) => {
        try {
            const response = await AuthorizedApi.postJson("camera", cameraModel);
            return response;
        } catch (error) {
            console.error("Nepodařilo se vytvořit kameru.", error);
            throw error;
        }
    },
    update: async (cameraModel) => {
        try {
            const response = await AuthorizedApi.putJson("camera", cameraModel)
            return response;
        } catch (error) {
            console.error("Nepodařilo se editovat kameru.", error);
            throw error;
        }
    },
    delete: async (cameraId) => {
        try {
            const response = await AuthorizedApi.deleteJson(`camera?cameraId=${cameraId}`);
            return response;
        } catch (error) {
            console.error("Nepodařilo se smazat kameru.", error);
            throw error;
        }
    },
    deleteVideo: async (cameraId, videoId) => {
        try {
            const respose = await AuthorizedApi.deleteJson(`camera/video?cameraId=${cameraId}&videoId=${videoId}`);
            return respose;
        } catch (error) {
            console.error("Nepodařilo se smazat video.", error);
            throw error;
        }
    },
    getImage: async (cameraId) => {
        try {
            const data = await AuthorizedApi.get(`camera/image?cameraId=${cameraId}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat obrázek.", error);
            throw error;
        }
    },
    deviceToken: async (deviceId) => {
        try {
            const data = await AuthorizedApi.getJson(`identity/Token?deviceId=${deviceId}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat token.", error);
            throw error;
        }
    },
    cameraValidation: (camera) => {
        let isValid = true;
        let validationErrors = { name: '' };

        if (camera.name.length < 4) {
            validationErrors.name = 'N\u00E1zev mus\u00ED obsahovat alespo\u0148 4 znaky.';
            isValid = false;
        }

        if (parseInt(camera.period) < 0) {
            validationErrors.period = 'Perioda mus\u00ED b\u00FDt v\u011Bt\u0161\u00ED jak 0.';
            isValid = false;
        }

        return { isValid, validationErrors };
    },
    uploadVideo: (cameraId, data) => {
        try {
            const response = AuthorizedApi.post(`camera/video-upload?cameraId=${cameraId}`, data)
            return response;
        } catch (error) {
            console.error("Nepodařilo se nahrát video.", error);
            throw error;
        }
    }
};

export default CameraService