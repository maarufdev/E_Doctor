(function () {
    const MANAGE_USER_BASE_URL = "ManageUser";
    const USER_STATUS = {
        ACTIVE: 1,
        ARCHIVE: 0,
        ALL: "*"
    };
    const MODEL_DIFINITIONS = {
        SAVE_USER: {
            userId: null,
            firstName: null,
            middleName: null,
            lastName: null,
            dob: null,
            statusId: 1,
            email: null,
            password: null,
        }
    }
    const URLS = {
        getManageUsers: `${MANAGE_USER_BASE_URL}/GetManageUsers`,
        saveManageUser: `${MANAGE_USER_BASE_URL}/SaveManageUser`,
        getManageUserById: `${MANAGE_USER_BASE_URL}/GetManageUserById`,
    }

    const stateHolders = {
        commands: {
            saveUser: null,
        },
        queries: {
            getManageUsers: {
                searchText: "",
                pageNumber: 1,
                pageSize: 10,
                userStatusId: null,
            }
        },
        manageUserPagination: {
            totalPages: 0,
        }
    }
    const elementHolders = {
        tables: {
            manageUserTableBody: "#manage-user-table-body",
        },
        buttons: {
            manageUserActions: {
                viewHistory: ".manage-user-view-history-btn",
                edit: ".manage-user-edit-btn",
                reset: ".manage-user-reset-btn",
                delete: ".manage-user-delete-btn",
                archive: ".manage-user-archive-btn",
                unarchive: ".manage-user-unarchive-btn"
            },
            newUserBtn: "#new-manage-user-btn",
        },
        manageUserFilters: {
            searchText: "#search-manage-account-text",
            filterStatus: "#filter-manage-account-select",
            searchBtn: "#seach-manage-account-btn",
        },
        paginations: {
            manageUserPagination: "#manage-user-pagination",
        },
        forms: {
            manageUser: {
                root: "#patient-account-md",
                title: "#patient-account-md-title",
                fields: {
                    firstName: "#user-acc-first-name",
                    middleName: "#user-acc-middle-name",
                    lastName: "#user-acc-last-name",
                    dob: "#user-acc-dob",
                    userStatusId: "#user-acc-status",
                    email: "#user-acc-email",
                    password: "#user-acc-password",
                },
                buttons: {
                    save: "#save-user-acc-btn",
                    close: "#patient-account-md-close-btn",
                }
            }
        }
    }

    const services = {
        initialize: function () {
            services.eventHandlers.renderManagedUsers();
            services.events.initMain();
            services.events.initManageUserForm();
        },
        eventHandlers: {
            toggleManageUserForm: function ({ toggle, title }) {
                const { manageUser: { root, title: titleElmt } } = elementHolders.forms;
                $(root).toggleClass("visible", toggle);
                $(titleElmt).text(title ?? "");
            },
            handleOnGetManageUserById: async function (userId) {
                const user = await services.apiService.getManageUserById(userId);
                const { fields } = elementHolders.forms.manageUser;

                const dateOfBirth = user.dateOfBirth.split("T")[0];
                $(fields.firstName).val(user.firstName);
                $(fields.middleName).val(user.middleName);
                $(fields.lastName).val(user.lastName);
                $(fields.dob).val(dateOfBirth);
                $(fields.userStatusId).val(user.userStatusId);
                $(fields.email).val(user.email);
                $(fields.password).attr("readonly", true);

                let userData = { ...user };
                userData = {...user, dateOfBirth: dateOfBirth }
                stateHolders.commands.saveUser = userData;

                this.toggleManageUserForm({ toggle: true, title: "Edit User Account" });
            },
            renderManagedUsers: async function () {
                const eventHandlers = this;
                const manageUsersResult = await services.apiService.getManagedUsers();
                const manageUserButtons = elementHolders.buttons.manageUserActions;

                stateHolders.manageUserPagination.totalPages = manageUsersResult.totalPages ?? 0;
                createPaginationElements();

                const manageUserList = manageUsersResult.items ?? [];

                const manageUsersTblBody = $(elementHolders.tables.manageUserTableBody);

                manageUsersTblBody.empty();

                manageUserList.map(user => {
                    const { userId, fullName, userStatusId } = user;
                    const $createdRow = createTableRow(user);

                    registerEvent(
                        $createdRow.find(manageUserButtons.viewHistory),
                        "click",
                        function (e) {
                            console.log("View History")
                        }
                    );

                    registerEvent(
                        $createdRow.find(manageUserButtons.edit),
                        "click",
                        () => eventHandlers.handleOnGetManageUserById(userId)
                    );

                    registerEvent(
                        $createdRow.find(manageUserButtons.reset),
                        "click",
                        function (e) {
                            console.log("Reset User Password")
                        }
                    );

                    registerEvent(
                        $createdRow.find(manageUserButtons.delete),
                        "click",
                        function (e) {
                            console.log("Delete User")
                        }
                    );

                    if (userStatusId == USER_STATUS.ACTIVE) {
                        registerEvent(
                            $createdRow.find(manageUserButtons.unarchive),
                            "click",
                            function (e) {
                                console.log("unarchive user")
                            }
                        );
                    }
                    else {
                        registerEvent(
                            $createdRow.find(manageUserButtons.archive),
                            "click",
                            function (e) {
                                console.log("archive user")
                            }
                        );
                    }

                    manageUsersTblBody.append($createdRow);
                });

                function createTableRow({
                    userId,
                    email,
                    fullName,
                    lastLoggedIn,
                    userStatus,
                    userStatusId,
                }) {
                    const $row = $("<tr>");
                    //$row.html(`
                    //    <td>${fullName}</td>
                    //    <td>${email}</td>
                    //    <td>
                    //        <span class="manage-user-status ${isUserActive(userStatusId)}" >${userStatus}</span>
                    //    </td>
                    //    <td>
                    //         <button class="btn btn-primary manage-user-view-history-btn" style="padding: 0.5rem;" title="View Login History">
                    //                <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                    //        </button>
                    //        <button class="btn manage-user-edit-btn" style="padding: 0.5rem;" title="Edit Account">
                    //            <svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.5L16.732 3.732z"></path></svg>
                    //        </button>
                    //        <button type="button" class="btn manage-user-btn manage-user-reset-btn" title="Reset Password">
                    //            <svg style ="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21.5 2v6h-6"></path><path d="M2.5 22v-6h6"></path><path d="M21.5 8a10 10 0 0 0-18.7-3.2M2.5 16a10 10 0 0 0 18.7 3.2"></path></svg>
                    //            </button>
                    //        <button class="btn btn-danger manage-user-delete-btn" style="padding: 0.5rem;" title="Delete Account">
                    //            <svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                    //            </button>
                    //        ${createArchiveOrUnArchiveButton(userStatusId)}
                    //    </td>
                    //    <td>${lastLoggedIn}</td>
                    //`);

                    $row.html(`
                        <td>${fullName}</td>
                        <td>${email}</td>
                        <td>
                            <span class="manage-user-status ${isUserActive(userStatusId)}" >${userStatus}</span>
                        </td>
                        <td>
                             <button class="btn btn-primary manage-user-view-history-btn" style="padding: 0.5rem;" title="View Login History">
                                    <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                            </button>
                            <button class="btn manage-user-edit-btn" style="padding: 0.5rem;" title="Edit Account">
                                <svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.5L16.732 3.732z"></path></svg>
                            </button>
                            <button type="button" class="btn manage-user-btn manage-user-reset-btn" title="Reset Password">
                                <svg style ="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21.5 2v6h-6"></path><path d="M2.5 22v-6h6"></path><path d="M21.5 8a10 10 0 0 0-18.7-3.2M2.5 16a10 10 0 0 0 18.7 3.2"></path></svg>
                                </button>
                            <button class="btn btn-danger manage-user-delete-btn" style="padding: 0.5rem;" title="Delete Account">
                                <svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                                </button>
                        </td>
                        <td>${lastLoggedIn}</td>
                    `);

                    return $row;
                }

                function isUserActive(statusId) {
                    return statusId == USER_STATUS.ACTIVE ? "active" : "";
                }

                function createArchiveOrUnArchiveButton(statusId) {
                    let button = "";
                    if (statusId == USER_STATUS.ACTIVE) {
                        button = `
                         <button type="button" class="btn manage-user-btn manage-user-archive-btn" title="Archive Account">
                                <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="20" height="5" x="2" y="3" rx="1"></rect><path d="M4 8v11a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8"></path><path d="M10 12l2 2 2-2"></path></svg>
                         </button>
                        `
                    }
                    else {
                        button = `
                            <button type="button" class="btn manage-user-btn manage-user-unarchive-btn" title="Re-Activate Account">
                                <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 2v10"></path><path d="m18.8 6.8-5.8 5.8"></path><path d="M22 17c0 4.4-3.6 8-8 8s-8-3.6-8-8 3.6-8 8-8c1 0 2 0.2 3 0.5"></path><path d="M16 11l-5.8 5.8"></path><path d="M22 7l-5.8 5.8"></path></svg>
                            </button>
                        `
                    }

                    return button;
                }

                function createPaginationElements() {
                    const { manageUserPagination } = stateHolders;
                    const $paginations = $(elementHolders.paginations.manageUserPagination);
                    $paginations.empty();

                    for (let i = 1; i <= manageUserPagination.totalPages; i++) {
                        const $option = $("<option>");
                        $option.val = i;
                        $option.text(i);
                        $paginations.append($option);
                    }

                    $paginations.val(stateHolders.queries.getManageUsers.pageNumber);
                }
            },
            saveUser: async function () {
            },
            handleOnCreateNewAccount: function () {
                const { buttons, forms } = elementHolders;
                this.clearManageUserForm();

                this.toggleManageUserForm({
                    toggle: true,
                    title: "Create Patient Account"
                })
            },
            clearManageUserForm: function () {
                const { manageUser: { fields } } = elementHolders.forms;
                $(fields.firstName).val("");
                $(fields.lastName).val("");
                $(fields.middleName).val("");
                $(fields.dob).val("");
                $(fields.userStatusId).val("1");
                $(fields.email).val("");
                $(fields.password).val("");

                const commands = stateHolders.commands;
                commands.saveUser = null;
            },
            clearManageUserCommand: function () {

            },
            populateUserForm: async function () {

            }
        },
        events: {
            initMain: function () {
                const { manageUserFilters, paginations, buttons } = elementHolders;
                const { queries: queryParams } = stateHolders;
                const eventHandlers = services.eventHandlers;

                registerEvent(
                    manageUserFilters.searchText,
                    "input",
                    function (event) {
                        queryParams.getManageUsers.searchText = event.target.value;
                    }
                );

                registerEvent(
                    manageUserFilters.filterStatus,
                    "change",
                    function (event) {
                        const filterStatusVal = event.target.value;
                        queryParams.getManageUsers.pageNumber = 1;
                        queryParams.getManageUsers.userStatusId = getFilterStatusValue(filterStatusVal);
                        services.eventHandlers.renderManagedUsers();
                    }
                );

                registerEvent(
                    manageUserFilters.searchBtn,
                    "click",
                    function (event) {
                        const filterStatusVal = $(manageUserFilters.filterStatus).val();
                        queryParams.getManageUsers.pageNumber = 1;
                        queryParams.getManageUsers.userStatusId = getFilterStatusValue(filterStatusVal);
                        services.eventHandlers.renderManagedUsers();
                    }
                );

                registerEvent(
                    paginations.manageUserPagination,
                    "change",
                    function (event) {
                        queryParams.getManageUsers.pageNumber = event.target.value;
                        services.eventHandlers.renderManagedUsers();
                    }
                );

                registerEvent(
                    buttons.newUserBtn,
                    "click",
                    function (event) {
                        eventHandlers.handleOnCreateNewAccount();
                    }
                );

                function getFilterStatusValue(filterStatusVal) {
                    return filterStatusVal == "*" ? null : filterStatusVal;
                }
            },
            initManageUserForm: function () {
                const { manageUser: { fields, buttons, root } } = elementHolders.forms;
                const { commands } = stateHolders;
                const eventHandlers = services.eventHandlers;

                registerEvent(
                    fields.firstName,
                    "input",
                    function (e) {
                        handleOnChangeInput(e);
                    }
                );
                registerEvent(
                    fields.middleName,
                    "input",
                    function (e) {
                        handleOnChangeInput(e);
                    }
                );
                registerEvent(
                    fields.lastName,
                    "input",
                    function (e) {
                        handleOnChangeInput(e);
                    }
                );
                registerEvent(
                    fields.dob,
                    "input",
                    function (e) {
                        handleOnChangeInput(e);
                    }
                );
                registerEvent(
                    fields.userStatusId,
                    "change",
                    function (e) {
                        handleOnChangeInput(e);
                    }
                );
                registerEvent(
                    fields.email,
                    "input",
                    function (e) {
                        handleOnChangeInput(e);
                    }
                );
                registerEvent(
                    fields.password,
                    "input",
                    function (e) {
                        handleOnChangeInput(e);
                    }
                );

                registerEvent(
                    buttons.save,
                    "click",
                    handleOnSaveAccount
                );

                registerEvent(
                    buttons.close,
                    "click",
                    () => {
                        eventHandlers.toggleManageUserForm({ toggle: false });
                        eventHandlers.clearManageUserCommand();
                    }
                );

                function handleOnChangeInput(event) {
                    let { value, name } = event.target;

                    if (name == "userStatusId") {
                        value = parseInt(value);
                    }

                    commands.saveUser = {
                        ...commands.saveUser,
                        [name]: value
                    }

                    console.log(commands.saveUser);
                }

                async function handleOnSaveAccount() {
                    const isCofirm = confirm("Are you sure to save user account?");
                    if (!isCofirm) return;

                    const result = await services.apiService.saveManageUser(commands.saveUser);
                    if (result == true) {
                        console.log(commands.saveUser);
                        setTimeout(() => {
                            eventHandlers.toggleManageUserForm({ toggle: false })
                        }, 500);

                        eventHandlers.renderManagedUsers();
                    }
                }
            },
        },
        apiService: {
            saveManageUser: async function (command) {
                return await apiFetch(
                    URLS.saveManageUser,
                    {
                        method: "POST",
                        body: command
                    }
                );
            },
            getManagedUsers: async function () {
                return await apiFetch(
                    URLS.getManageUsers,
                    {
                        params: stateHolders.queries.getManageUsers
                    }
                );
            },
            getManageUserById: async function (userId) {
                return await apiFetch(
                    URLS.getManageUserById,
                    {
                        params: {
                            userId: userId
                        }
                    }
                );
            },
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();