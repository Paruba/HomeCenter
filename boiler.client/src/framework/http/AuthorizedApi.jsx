import Cookies from 'js-cookie';
import config from '../../../config';

const AuthorizedApi = {
    baseUrl: () => { return config.apiUrl },
    postJson: async (route, body) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`
                },
                body: JSON.stringify(body)
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            try {
                const data = await response.json();
                return data;
            } catch (jsonError) {
                return response;
            }
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    },
    getJson: async (route) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'GET',
                headers: {
                    'Content-Type': "application/json",
                    'Authorization': `Bearer ${accessToken}`
                }
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            try {
                const data = await response.json();
                return data;
            } catch (jsonError) {
                return response;
            }
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    },
    deleteJson: async (route) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'DELETE',
                headers: {
                    'Content-Type': "application/json",
                    'Authorization': `Bearer ${accessToken}`
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            try {
                const data = await response.json();
                return data;
            } catch (jsonError) {
                return response;
            }
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    },

    putJson: async (route, body) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`
                },
                body: JSON.stringify(body)
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            try {
                const data = await response.json();
                return data;
            } catch (jsonError) {
                return response;
            }
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    },
    put: async (route, body) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${accessToken}`
                },
                body: JSON.stringify(body)
            });
            return response;
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    },
    getBlob: async (route) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${accessToken}`,
                },
                responseType: 'blob',
            });
            const blob = await response.blob();
            return blob;
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    },
    get: async (route) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${accessToken}`,
                },
            });

            return response;
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    },
    post: async (route, body) => {
        try {
            const accessToken = Cookies.get('AccessToken');
            const response = await fetch(AuthorizedApi.baseUrl() + route, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                },
                body: body
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            try {
                const data = await response.json();
                return data;
            } catch (jsonError) {
                return response;
            }
        } catch (error) {
            console.log("Nepovedlo se odeslat požadavek na server.", error);
            throw error;
        }
    }
};

export default AuthorizedApi;