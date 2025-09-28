(function () {
    const DIAGNOSIS_BASE_URL = "Diagnosis";
    const URLS = {
        getSymptoms: `${DIAGNOSIS_BASE_URL}/GetSymptoms`,
        runDiagnosis: `${DIAGNOSIS_BASE_URL}/RunDiagnosis`,
        getDiagnosis: `${DIAGNOSIS_BASE_URL}/GetDiagnosis`,
        getDiagnosisById: `${DIAGNOSIS_BASE_URL}/GetDiagnosisById`,
        importRulesConfiguration: `${DIAGNOSIS_BASE_URL}/ImportRulesConfiguration`
    }
    const stateHolders = {
        symptoms: [],
        selectedSymptoms: [],
        importedRuleFile: null,
        searchParams: {
            pageNumber: 1,
            pageSize: 10
        },
        diagnosisPaginatedResult: {
            totalPages: 0,
        },
    }
    const elementHolders = {
        common: {
            buttons: {
                newConsultation: "#new-consultation-btn",
                showImportFileModal: "#import-rules-btn",
            },
            diagnosisPagination: "#diagnosis-pagination",
        },
        tables: {
            diagnosis: {
                root: "",
                body: "#history-table-body"
            }
        },
        modals: {
            diagnosis: {
                root: "#diagnosis-result-modal",
                buttons: {
                    close: "#close-diagnosis-result-modal"
                },
                details: {
                    result: "#diagnosis-result",
                    description: "#diagnosis-description",
                    prescription: "#diagnosis-prescriptions",
                    notes: "#diagnosis-notes",
                }
            },
            importRule: {
                root: "#import-rules-modal",
                fields: {
                    fileInput: "#import-rule-config-input"
                },
                buttons: {
                    saveRules: "#save-rules-config",
                    close: "#close-rules-modal",
                }

            },
        },
    }
    const services = {
        initialize: function () {
            services.eventHandlers.renderDiagnosisTable();
            services.events.initCommonEvents();
            services.events.initDiagnosis();
            services.events.initImportRulesevent();
        },
        eventHandlers: {
            //Consultation Sections
            toggleDiagnosisModal: function (toOpen) {
                const modal = elementHolders.modals.diagnosis;
                $(modal.root).toggleClass("visible", toOpen);
            },
            renderDiagnosisTable: async function () {
                const { diagnosis } = elementHolders.tables;
                const $diagnosisTblBody = $(diagnosis.body);
                $diagnosisTblBody.empty();

                const result = await services.apiService.getDiagnosis() ?? [];
                const diagnosisReponse = result.items ?? [];

                diagnosisReponse.forEach(item => {
                    const { diagnosisId } = item;
                    const $tr = $(`
                        <tr>
                            <td>${convertDateTimeToLocal(item.diagnoseDate)}</td>
                            <td style="white-space: normal;">${item.symptoms}</td>
                            <td>${item.illnessName}</td>
                            <td><a href="#" class="btn-view-diagnosis">View Diagnosis</a></td>
                        </tr>
                    `);

                    registerEvent(
                        $tr.find(".btn-view-diagnosis"),
                        "click",
                        async function (event) {
                            event.preventDefault();

                            if (diagnosisId) {
                                const result = await services.apiService.getDiagnosisById(diagnosisId);
                                services.eventHandlers.handleOnShowDiagnosisResult(result);
                            }

                        }
                    )
                    $diagnosisTblBody.append($tr);
                });

                stateHolders.diagnosisPaginatedResult.totalPages = result.totalPages;

                this.createPaginations();
            },
            createPaginations: function () {
                const { common } = elementHolders;
                const { totalPages } = stateHolders.diagnosisPaginatedResult;

                const $pagination = $(common.diagnosisPagination);
                $pagination.empty();

                for (let i = 1; i <= totalPages; i++) {
                    const $option = $(`<option>`);
                    $option.val = i;
                    $option.text(i);
                    $pagination.append($option);
                }

                $pagination.val(stateHolders.searchParams.pageNumber);
            },
            populateDiagnosisResult: function ({ result, description, prescription, notes }) {
                const { diagnosis } = elementHolders.modals;

                $(diagnosis.details.result).text(" ");
                $(diagnosis.details.prescription).text(" ");
                $(diagnosis.details.description).text(" ");
                $(diagnosis.details.notes).text(" ");

                if (result) {
                    $(diagnosis.details.result).text(result ?? " ");
                    $(diagnosis.details.prescription).text(prescription ?? " ");
                    $(diagnosis.details.description).text(description ?? " ");
                    $(diagnosis.details.notes).text(notes ?? " ");
                }
            },
            handleOnShowDiagnosisResult: function (result) {
                this.populateDiagnosisResult(result);
                this.toggleDiagnosisModal(true);
            },
            // Import Rules Sections
            toggleImportFileModal: function (toOpen) {
                const modal = elementHolders.modals.importRule;
                $(modal.root).toggleClass("visible", toOpen);
            },
            handleOnSaveRules: async function () {
                const { importRule } = elementHolders.modals
                const fileInput = stateHolders.importedRuleFile;

                if (!fileInput) {
                    alert("Please Select a file to import.");
                    return;
                }

                const formData = new FormData();
                formData.append("file", fileInput);

                const result = await services.apiService.importFile(formData);

                if (!result) return;

                stateHolders.importedRuleFile = null;

                $(importRule.fields.fileInput).val("");

                this.toggleImportFileModal(false);

                this.renderDiagnosisTable();
            },
        },
        events: {
            initCommonEvents: function () {
                registerEvent(
                    elementHolders.common.buttons.newConsultation,
                    "click",
                    function (event) {
                        window.location.href = "?tabName=Consultation";
                    }
                );
            },
            initDiagnosis: function () {
                const { diagnosis } = elementHolders.modals;

                registerEvent(
                    diagnosis.buttons.close,
                    "click",
                    function (event) {
                        services.eventHandlers.toggleDiagnosisModal(false);
                    }
                );
                registerEvent(
                    elementHolders.common.diagnosisPagination,
                    "change",
                    function (event) {
                        const value = event.target.value;
                        stateHolders.searchParams.pageNumber = value;
                        services.eventHandlers.renderDiagnosisTable();
                    }
                );
            },
            initImportRulesevent: function () {
                const { importRule } = elementHolders.modals;
                const { common } = elementHolders;

                registerEvent(
                    importRule.buttons.saveRules,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnSaveRules();
                    }
                );

                registerEvent(
                    importRule.fields.fileInput,
                    "change",
                    function (event) {
                        const jsonFile = event.target.files[0];

                        if (!jsonFile) {
                            event.preventDefault();
                            return;
                        }

                        const splitFileName = jsonFile.name.split(".");
                        let fileExt = splitFileName[splitFileName.length - 1];

                        if (fileExt) {
                            fileExt = fileExt.toLowerCase();
                        }

                        if (fileExt != "json") {
                            event.preventDefault();
                            event.target.value = "";

                            alert("File imported is not a json file.");
                            return;
                        }

                        stateHolders.importedRuleFile = jsonFile;
                    }
                );

                registerEvent(
                    common.buttons.showImportFileModal,
                    "click",
                    function (event) {
                        services.eventHandlers.toggleImportFileModal(true);
                    }
                );

                registerEvent(
                    importRule.buttons.close,
                    "click",
                    function (event) {
                        services.eventHandlers.toggleImportFileModal(false);
                    }
                );
            },
        },
        apiService: {
            getSymptoms: async function () {
                return await apiFetch(URLS.getSymptoms);
            },
            getDiagnosis: async function () {
                return await apiFetch(
                    URLS.getDiagnosis,
                    {
                        params: stateHolders.searchParams
                    });
            },
            runDiagnosis: async (command) => {
                return await apiFetch(
                    URLS.runDiagnosis,
                    {
                        body: command,
                        method: "POST"
                    },
                );
            },
            importFile: async (formData) => {
                return await apiFetch(
                    URLS.importRulesConfiguration,
                    {
                        body: formData,
                        method: "POST"
                    },
                );
            },
            getDiagnosisById: async (diagnosisId) => {
                return await apiFetch(
                    URLS.getDiagnosisById,
                    {
                        params: {
                            diagnosisId: diagnosisId
                        },
                    },
                );
            },
        }
    };
    document.addEventListener("DOMContentLoaded", services.initialize);
})();