import AuthorizedApi from "../../../framework/http/AuthorizedApi";

const UserSerivce = {
    getUsers: async (searchValue, page, rowsPerPage) => {
        try {
            const take = rowsPerPage;
            const skip = page * take;
            const data = await AuthorizedApi.getJson(`identity/users?searchValue=${searchValue}&skip=${skip}&take=${take}`)
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat uživatele.", error);
            throw error;
        }
    },
    getUser: async (id) => {
        try {
            const data = await AuthorizedApi.getJson(`identity/user?userId=${id}`)
            return data;
        } catch (error) {
            console.error("Nepodařilo se získat uživatele.", error);
            throw error;
        }
    },
    delete: async (id) => {
        try {
            const data = await AuthorizedApi.deleteJson(`identity/user?userId=${id}`)
            return data;
        } catch (error) {
            console.error("Nepodařilo se vymazat uživatele.", error);
            throw error;
        }
    },
    changePassword: async (passwordForm, userId) => {
        try {
            const data = await AuthorizedApi.putJson(`identity/change-password?userId=${userId}`, passwordForm)
            return data;
        } catch (error) {
            console.error("Nepodařilo se změnit heslo.", error);
            throw error;
        }
    },
    registrationValidation: (registrationModel) => {
        let isValid = true;
        let validationErrors = { username: '', password: '', repeatedPassword: '' };

        // Email validation (simple regex for example)
        if (!/\S+@\S+\.\S+/.test(registrationModel.username)) {
            validationErrors.username = 'Pros\u00EDm zadejte validn\u00ED email.';
            isValid = false;
        }

        // Password validation
        if (registrationModel.password.length < 8) {
            validationErrors.password = 'Heslo mus\u00ED b\u00FDt del\u0161\u00ED jak 8 znak\u00F9.';
            isValid = false;
        }

        // Repeat password validation
        if (registrationModel.password !== registrationModel.repeatedPassword) {
            validationErrors.repeatedPassword = 'Hesla se mus\u00ED shodovat.';
            isValid = false;
        }

        return { isValid, validationErrors };
    }
};

export default UserSerivce;