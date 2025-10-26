(function () {
    const USER_ACTIVITY_BASE_URL = "UserActivity";

    const URLS = {
        getPatientUserActivities: `${USER_ACTIVITY_BASE_URL}/GetPatientUserActivities`,
    }
    const stateHolders = {
        getUserActivity: {
            query: {
                searchText: "",
                pageSize: 10,
                pageNumber: 1,
            },
            totalPages: 0 
        }
    }
    const elementHolders = {
        table: {
            userActivity: {
                body: "#user-activity-tbl-body",
                pagination: "#user-activity-pagination"
            }
        },
        fields: {
            search: "#search-user-activity-text",
        },
        buttons: {
            searchBtn: "#seach-user-btn"
        }
    }

    const services = {
        initialize: async function () {
            services.eventHandlers.renderUserAcitivity();
            services.events.initSearchActivityEvent();
        },
        eventHandlers: {
            renderUserAcitivity: async function () {
                const { table } = elementHolders;
                const result = await services.apiService.getUserActivity();
                const activities = result?.items ?? [];

                const $tableBody = $(table.userActivity.body);
                $tableBody.empty();

                activities.map(activity => {
                    const $tr = $("<tr>");

                    const $dateCol = $("<td>");
                    const $userCol = $("<td>");
                    const $activityTypeCol = $("<td>");
                    const $descCol = $("<td>");

                    const $activityTypeSpan = $(`
                        <span class='activity-action-text'>
                    `);
                    $activityTypeSpan.text(activity.activityType);


                    $dateCol.text(activity.displayDate);
                    $userCol.text(activity.user);
                    $activityTypeCol.append($activityTypeSpan);
                    $descCol.text(activity.description);

                    $tr.append($dateCol);
                    $tr.append($userCol);
                    $tr.append($activityTypeCol);
                    $tr.append($descCol);

                    $tableBody.append($tr);
                });

                stateHolders.getUserActivity.totalPages = result.totalPages;
                createPagination();

                function createPagination() {
                    const $pagination = $(elementHolders.table.userActivity.pagination);
                    $pagination.empty();

                    for (let i = 1; i <= stateHolders.getUserActivity.totalPages; i++) {
                        const $option = $(`<option>`);
                        $option.val = i;
                        $option.text(i);
                        $pagination.append($option);
                    }
                    $pagination.val(stateHolders.getUserActivity.query.pageNumber);
                }
            },
        },
        events: {
            initSearchActivityEvent: function () {
                const { fields, buttons, table: { userActivity } } = elementHolders;

                registerEvent(
                    fields.search,
                    "input",
                    handleOnSeachInputChange
                );
                registerEvent(
                    buttons.searchBtn,
                    "click",
                    function (e) {
                        stateHolders.getUserActivity.query.pageNumber = 1;
                        services.eventHandlers.renderUserAcitivity();
                    }
                );
                registerEvent(
                    userActivity.pagination,
                    "change",
                    function (e) {
                        handleOnSeachInputChange(e);
                        services.eventHandlers.renderUserAcitivity();
                    }
                );

                function handleOnSeachInputChange(event) {
                    let { name, value } = event.target;

                    if (name === "pageNumber") {
                        value = parseInt(value);
                    }
                    
                    stateHolders.getUserActivity.query = {
                        ...stateHolders.getUserActivity.query,
                        [name]: value
                    };

                }
            }
        },
        apiService: {
            getUserActivity: async function () {
                return await apiFetch(
                    URLS.getPatientUserActivities,
                    {
                        params: stateHolders.getUserActivity.query,
                    }
                );
            }
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();