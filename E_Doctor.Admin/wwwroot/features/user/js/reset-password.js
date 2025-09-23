class ResetPasswordService {
    constructor() {

        this.loginCommand = {
            userName: null,
            newPassword: null
        };

        this.userNameElmt = null;
        this.newPasswordElmt = null;
        this.resetBtn = null;
    }

    initialize() {
        document.addEventListener("DOMContentLoaded", () => {
            this.userNameElmt = "#username";
            this.newPasswordElmt = "#new-password";
            this.resetBtn = "#reset-password-btn";

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
            this.newPasswordElmt,
            "input",
            function (event) {
                login.loginCommand.newPassword = event.target.value;
            }
        );

        registerEvent(
            this.resetBtn,
            "click",
            async function (event) {
                event.preventDefault();

                const { loginCommand: command } = login;

                let errors = [];

                if (!command.userName || command.userName.trim() === "") {
                    errors.push("Username/Email is required.");
                }

                if (!command.newPassword || command.newPassword.trim() === "") {
                    errors.push("New Password is required.");
                }

                if (errors.length > 0) {
                    alert(errors.join("\n"));
                    return;
                }

                const result = await login.apiService().forgot(command);

                if (result) {
                    alert("Password has been reset successfully.")
                    window.location.href = "/Account/Login";
                }
            }
        );
    }

    apiService() {
        return {
            forgot: async function (command) {
                return await apiFetch(
                    "/Account/ResetPassword",
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
    const service = new ResetPasswordService();
    service.initialize();
}());
