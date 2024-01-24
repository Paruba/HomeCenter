import AuthorizedApi from '../../../framework/http/AuthorizedApi';
import HttpApi from '../../../framework/http/HttpApi';

const AuthService = {
    login: async (credentials) => {
        try {
            const data = await HttpApi.postJson('identity/login', credentials);
            return data;
        } catch (error) {
            console.error("Login error", error);
            throw error;
        }
    },
    registration: async (refistrationForm) => {
        try {
            const data = await AuthorizedApi.postJson('identity/registration', refistrationForm);
            return data;

        } catch (error) {
            console.log("Registration error", error);
        }
    },
    deviceToken: async (deviceId, device) => {
        try {
            const data = await AuthorizedApi.getJson(`identity/Token?deviceId=${deviceId}&device=${device}`);
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat token.", error);
            throw error;
        }
    },
    getRoles: async () => {
        try {
            const response = await AuthorizedApi.getJson("identity/roles");
            return response;
        } catch (error) {
            console.error("Nepodařilo se načíst role.", error);
            throw error;
        }
    },
    getUser: async (id) => {
        try {
            const response = await AuthorizedApi.getJson(`identity/user-detail?userId=${id}`);
            return response
        } catch (error) {
            console.error("Nepodařilo se načíst uživatele.", error);
            throw error;
        }
    },
    editUser: async (user) => {
        try {
            const response = await AuthorizedApi.put("identity/user", user);
            return response;
        } catch (error) {
            console.error("Nepodařilo se editovat uživatele.", error);
            console.error(error);
        }
    }
};

export default AuthService;