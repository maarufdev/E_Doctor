class LoginService {
    constructor() {

        this.loginCommand = {
            userName: null,
            password: null
        };

        this.userNameElmt = null;
        this.passwordElmt = null;
        this.submitBtn = null;
    }

    initialize() {
        document.addEventListener("DOMContentLoaded", () => {
            this.userNameElmt = "#login-username";
            this.passwordElmt = "#login-password";
            this.submitBtn = "#login-submit-btn";

            this.registerEvents();
        });
    }

    registerEvents() {
        const login = this;

        registerEvent(
            this.userNameElmt,
            "input",
            function (event) {
                login.loginCommand.userName = event.target.value;
            }
        );

        registerEvent(
            this.passwordElmt,
            "input",
            function (event) {
                login.loginCommand.password = event.target.value;
            }
        );

        registerEvent(
            this.submitBtn,
            "click",
            async function (event) {
                event.preventDefault();

                const { loginCommand: command } = login;

                let errors = [];

                if (!command.userName || command.userName.trim() === "") {
                    errors.push("Username is required.");
                }

                if (!command.password || command.password.trim() === "") {
                    errors.push("Password is required.");
                }

                if (errors.length > 0) {
                    alert(errors.join("\n"));   
                    return;                    
                }

                const result = await login.apiService().login(command);

                if (result) {
                    window.location.href = "/UserDashboard";
                }
            }
        );
    }

    apiService() {
        return {
            login: async function (command) {
                return await apiFetch(
                    "/Account/Login",
                    {
                        method: "POST",
                        body: command
                    }
                )
            }
        }
    }
}

(function () {
    const login = new LoginService();
    login.initialize();
}());
