import AuthorizedApi from "../../../framework/http/AuthorizedApi";

const FaceService = {
    getFaceDetection: async (videoIdsQuery, cameraId) => {
        try {
            const data = await AuthorizedApi.getJson(`face?cameraId=${cameraId}&videoIds=${videoIdsQuery}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat konfiguraci kamery.", error);
            throw error;
        }
    },
    findPeople: async (videoIdsQuery, cameraId, data) => {
        try {
            const response = await AuthorizedApi.post(`face?cameraId=${cameraId}&videoIds=${videoIdsQuery}`, data);
            return response;
        } catch (error) {
            console.error("Nepodařilo se vytvořit kameru.", error);
            throw error;
        }
    },
    getDetectedFaces: async (videoId, cameraId) => {
        try {
            const data = await AuthorizedApi.getJson(`face/detected-faces?cameraId=${cameraId}&videoId=${videoId}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat konfiguraci kamery.", error);
            throw error;
        }
    }
}

export default FaceService