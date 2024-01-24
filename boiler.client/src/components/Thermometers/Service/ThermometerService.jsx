import AuthorizedApi from '../../../framework/http/AuthorizedApi';

const ThermometerService = {
    getThermometers: async (searchValue, page, rowsPerPage) => {
        try {
            const take = rowsPerPage;
            const skip = page * take;
            const data = await AuthorizedApi.getJson(`thermometer?searchValue=${searchValue}&skip=${skip}&take=${take}`)
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat teploměry.", error);
            throw error;
        }
    },
    create: async (thermometerModel) => {
        try {
            const response = await AuthorizedApi.postJson("thermometer", thermometerModel);
            return response;
        } catch (error) {
            console.error("Nepodařilo se vytvořit teploměr.", error);
            throw error;
        }
    },
    delete: async (thermometerId) => {
        try {
            const response = await AuthorizedApi.deleteJson(`thermometer?thermometerId=${thermometerId}`);
            return response;
        } catch (error) {
            console.error("Nepodařilo se smazat teploměr.", error);
            throw error;
        }
    },
    termometerValidation: (termometer) => {
        let isValid = true;
        let validationErrors = { name: ''};
        
        // name validation
        if (termometer.name.length < 4) {
            validationErrors.name = 'N\u00E1zev mus\u00ED obsahovat alespo\u0148 4 znaky.';
            isValid = false;
        }

        return { isValid, validationErrors };
    }
};

export default ThermometerService