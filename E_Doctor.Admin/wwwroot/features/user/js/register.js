class RegisterService {
    constructor() {
        this.registerCommand = {
            userName: null,
            password: null,
            firstName: null,
            middleName: null,
            lastName: null,
            dateOfBirth: null
        };

        // Element selectors
        this.userNameElmt = null;
        this.passwordElmt = null;
        this.firstNameElmt = null;
        this.middleNameElmt = null;
        this.lastNameElmt = null;
        this.dobElmt = null;
        this.submitBtn = null;
    }

    initialize() {
        document.addEventListener("DOMContentLoaded", () => {
            // Map selectors to your HTML
            this.userNameElmt = "#reg-username";
            this.passwordElmt = "#reg-password";
            this.firstNameElmt = "#reg-firstname";
            this.middleNameElmt = "#reg-middlename";
            this.lastNameElmt = "#reg-lastname";
            this.dobElmt = "#reg-dob";
            this.submitBtn = "#register-form button[type='submit']";

            this.registerEvents();
        });
    }

    registerEvents() {
        const reg = this;

        registerEvent(this.userNameElmt, "input", e => reg.registerCommand.userName = e.target.value);
        registerEvent(this.passwordElmt, "input", e => reg.registerCommand.password = e.target.value);
        registerEvent(this.firstNameElmt, "input", e => reg.registerCommand.firstName = e.target.value);
        registerEvent(this.middleNameElmt, "input", e => reg.registerCommand.middleName = e.target.value);
        registerEvent(this.lastNameElmt, "input", e => reg.registerCommand.lastName = e.target.value);
        registerEvent(this.dobElmt, "input", e => reg.registerCommand.dateOfBirth = e.target.value);

        registerEvent(this.submitBtn, "click", async function (event) {
            event.preventDefault();

            const { registerCommand: command } = reg;

            let errors = [];

            if (!command.userName || command.userName.trim() === "") {
                errors.push("Username is required.");
            }
            if (!command.password || command.password.trim() === "") {
                errors.push("Password is required.");
            }
            if (!command.firstName || command.firstName.trim() === "") {
                errors.push("First name is required.");
            }
            if (!command.lastName || command.lastName.trim() === "") {
                errors.push("Last name is required.");
            }
            if (!command.dateOfBirth || command.dateOfBirth.trim() === "") {
                errors.push("Date of birth is required.");
            }

            if (errors.length > 0) {
                alert(errors.join("\n"));
                return;
            }

            const result = await reg.apiService().register(command);

            if (result) {
                window.location.href = "/Account/Login";
            } 
        });
    }

    apiService() {
        return {
            register: async function (command) {
                return await apiFetch(
                    "/Account/Register",
                    {
                        method: "POST",
                        body: command
                    }
                );
            }
        };
    }
}

// Initialize on load
(function () {
    const reg = new RegisterService();
    reg.initialize();
}());
