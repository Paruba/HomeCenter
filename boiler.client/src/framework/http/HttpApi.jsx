import config from "../../../config";

const HttpApi = {
    baseUrl: () => { return config.apiUrl },
    postJson: async (route, body) => {
        try {
            const response = await fetch(HttpApi.baseUrl() + route, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
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

        }
    }
};

export default HttpApi;