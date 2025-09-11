(function () {
    const SYMPTOM_BASE_URL = "AdminSetting";

    const URLS = {
        getSymptoms: `${SYMPTOM_BASE_URL}/GetSymptoms`,
        getSymptomById: `${SYMPTOM_BASE_URL}/GetSymptomById`,
        saveSymptom: `${SYMPTOM_BASE_URL}/SaveSymptom`,
        deleteSymptom: `${SYMPTOM_BASE_URL}/RemoveSymptom`,
    }
    const stateHolders = {
        commandQueries: {
            saveSymptom: {
                symptomId: null,
                symptomName: ""
            },
            symptomId: null,
        },
        searchParams: {
            searchText: "",
            pageNumber: 1,
            pageSize: 10
        },
        symptomsPaginatedResult: {
            totalPages: 0,
        }
    }
    const elementHolders = {
        searchSection: {
            searchInput: "#symptom-search-input",
            searchBtn: "#symptom-search-btn",
            paginationSelect: "#symptoms-pagination"
        },
        table: {
            symptomsTblBody: "#symptoms-table-body",
            actionBtns: {
                edit: "",
                delete: ""
            },
        },
        modals: {
            symptomsModal: {
                root: "#symptom-modal",
                title: "#symptom-modal-title",
                fields: {
                    symptomId: "#symptom-id-input",
                    symptomName: "#symptom-name-input"
                },
                buttons: {
                    save: "#save-symptom-btn",
                    close: "#close-symptom-modal-btn"
                },
            },
        },
        buttons: {
            newSymptom: "#new-symptom-btn"
        },
    }

    const services = {
        initialize: function () {
            services.events.initMain();
            services.events.searchEvent();
            services.eventHandlers.renderSymptomsTable();
        },
        eventHandlers: {
            toggleSymptomsModal: function (toOpen, toEdit) {
                const { modals } = elementHolders;
                $(modals.symptomsModal.root).toggleClass("visible", toOpen);
                $(modals.symptomsModal.title).text(toEdit ? "Edit Symptom" : "New Symptom");
            },
            populateSymptomForm: function (symptom) {
                if (IsNullUndefinedEmpty(symptom)) return;

                const { symptomsModal } = elementHolders.modals;
                $(symptomsModal.fields.symptomId).val(symptom.symptomId ?? "");
                $(symptomsModal.fields.symptomName).val(symptom.symptomName ?? "");
            },
            handleOnClickEditSymptom: async function (symptomId) {
                const { apiService, eventHandlers } = services;
                const symptom = await apiService.getSymptomById(symptomId);
                this.populateSymptomForm(symptom);

                eventHandlers.toggleSymptomsModal(true, true);
            },
            createPaginations: function () {
                const { searchSection } = elementHolders;
                const { totalPages } = stateHolders.symptomsPaginatedResult;

                const $pagination = $(searchSection.paginationSelect);
                $pagination.empty();

                for (let i = 1; i <= totalPages; i++) {
                    const $option = $(`<option>`);
                    $option.val = i;
                    $option.text(i);
                    $pagination.append($option);
                }

                $pagination.val(stateHolders.searchParams.pageNumber);
            },
            renderSymptomsTable: async function () {
                const { eventHandlers } = services;

                const { table } = elementHolders;
                const symptomsTableBody = document.querySelector(table.symptomsTblBody);

                if (IsNullUndefinedEmpty(symptomsTableBody)) {
                    return;
                }

                const result = await services.apiService.getSymptoms();

                const symptoms = result.items ?? [];

                symptomsTableBody.innerHTML = "";

                symptoms.forEach(symptom => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${symptom.symptomName}</td>
                        <td class="actions-cell">
                            <button class="btn btn-warning edit-symptom-btn" style="padding: 0.5rem;" data-symptom-id="${symptom.symptomId}" title="Edit"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.5L16.732 3.732z"></path></svg></button>
                            <button class="btn btn-danger delete-symptom-btn" style="padding: 0.5rem;" data-symptom-id="${symptom.symptomId}" title="Delete"><svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg></button>
                        </td>
                    `;

                    row.querySelector('.edit-symptom-btn').addEventListener('click', (e) => {
                        const { symptomId } = e.currentTarget.dataset;
                        eventHandlers.handleOnClickEditSymptom(symptomId);
                    });

                    row.querySelector('.delete-symptom-btn').addEventListener('click', (e) => {
                        const { symptomId } = e.currentTarget.dataset;
                        if (!symptomId) return;

                        const isYes = confirm("Are you sure? Deleting a symptom will also affect illness rules.");

                        if (isYes) {
                            this.handleOnDeleteSymptom(symptomId);
                        }
                    });
                    symptomsTableBody.appendChild(row);
                });

                stateHolders.symptomsPaginatedResult.totalPages = result.totalPages;

                this.createPaginations();
            },
            handleOnSaveSymptom: async function () {
                const { symptomsModal } = elementHolders.modals;
                const { apiService } = services;

                const $symptomIdField = $(symptomsModal.fields.symptomId);
                const $symptomNameField = $(symptomsModal.fields.symptomName);

                const symptomId = $symptomIdField.val();
                const symptomName = $symptomNameField.val();

                const command = {
                    symptomId: symptomId ? parseInt(symptomId) : 0,
                    symptomName: symptomName
                };

                if (IsNullUndefinedEmpty(symptomName)) return;

                const isSuccess = await apiService.saveSymptom(command);

                if (isSuccess) {
                    stateHolders.pageNumber = 1;
                    await this.renderSymptomsTable();
                    this.toggleSymptomsModal(false);
                    this.clearSymptomModal();
                }
            },
            handleOnDeleteSymptom: async function (symptomId) {
                const { apiService } = services;
                const isDeleted = await apiService.deleteSymptom(symptomId);

                if (!isDeleted) return;

                stateHolders.pageNumber = 1;
                await this.renderSymptomsTable();
            },
            clearSymptomModal: function () {
                const { symptomsModal } = elementHolders.modals;
                $(symptomsModal.fields.symptomId).val("");
                $(symptomsModal.fields.symptomName).val("");
            }
        },
        events: {
            initMain: function () {
                const { eventHandlers } = services;
                const { buttons, modals } = elementHolders;

                $(buttons.newSymptom).off("click").on("click", function () {
                    eventHandlers.toggleSymptomsModal(true);
                    eventHandlers.clearSymptomModal();
                });
                $(modals.symptomsModal.buttons.close).off("click").on("click", function () {
                    eventHandlers.toggleSymptomsModal(false);
                });

                $(modals.symptomsModal.buttons.save).off("click").on("click", function () {
                    eventHandlers.handleOnSaveSymptom();
                });
            },
            searchEvent: function () {
                const { searchSection } = elementHolders;

                registerEvent(
                    searchSection.searchBtn,
                    "click",
                    function () {
                        const searchValue = $(searchSection.searchInput).val();
                        stateHolders.searchParams.searchText = searchValue;
                        stateHolders.searchParams.pageNumber = 1;
                        services.eventHandlers.renderSymptomsTable();
                    }
                );

                registerEvent(
                    searchSection.searchBtn,
                    "input",
                    function () {
                        const searchValue = $(searchSection.searchInput).val();
                        stateHolders.searchParams.searchText = searchValue;
                    }
                );

                registerEvent(
                    searchSection.paginationSelect,
                    "change",
                    function (event) {
                        const value = event.target.value;
                        stateHolders.searchParams.pageNumber = value;
                        services.eventHandlers.renderSymptomsTable();
                    }
                );
            }
        },
        apiService: {
            getSymptoms: async () => {
                return await apiFetch(
                    URLS.getSymptoms,
                    {
                        params: stateHolders.searchParams
                    }
                );
            },
            getSymptomById: async (symptomId) => {
                const symptom = await apiFetch(URLS.getSymptomById, {
                    params: {
                        symptomId: symptomId 
                    },
                })

                return symptom;
            },
            saveSymptom: async (command) => {
                const isSuccess = await apiFetch(URLS.saveSymptom, {
                    method: "POST",
                    body: command
                });

                return isSuccess;
            },
            deleteSymptom: async (symptomId) => {
                return await apiFetch(URLS.deleteSymptom, {
                    method: "DELETE",
                    params: {
                        symptomId: symptomId,
                    }
                });
            }
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();