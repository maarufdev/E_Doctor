(function () {
    const DIAGNOSIS_BASE_URL = "AdminDiagnosis";
    const SETTINGS_BASE_URL = "AdminSetting";
    const URLS = {
        getSymptoms: `${SETTINGS_BASE_URL}/GetIllnessSymptoms`,
        runDiagnosis: `${DIAGNOSIS_BASE_URL}/RunDiagnosis`,
        getDiagnosis: `${DIAGNOSIS_BASE_URL}/GetDiagnosis`,
        getDiagnosisById: `${DIAGNOSIS_BASE_URL}/GetDiagnosisById`,
    }
    const stateHolders = {
        symptoms: [],
        selectedSymptoms: [],
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
                diagnosisInfo: {
                    result: "#diagnosis-result",
                    description: "#diagnosis-description",
                    prescription: "#diagnosis-prescriptions",
                    notes: "#diagnosis-notes",
                }
            }
        },
    }
    const services = {
        initialize: function () {
            services.eventHandlers.renderDiagnosisTable();
            services.events.initDiagnosis();
            services.events.initCommonEvents();
        },
        eventHandlers: {
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

                $(diagnosis.diagnosisInfo.result).text(" ");
                $(diagnosis.diagnosisInfo.prescription).text(" ");
                $(diagnosis.diagnosisInfo.description).text(" ");
                $(diagnosis.diagnosisInfo.notes).text(" ");

                if (result) {
                    $(diagnosis.diagnosisInfo.result).text(result ?? " ");
                    $(diagnosis.diagnosisInfo.prescription).text(prescription ?? " ");
                    $(diagnosis.diagnosisInfo.description).text(description ?? " ");
                    $(diagnosis.diagnosisInfo.notes).text(notes ?? " ");
                } 
            },
            handleOnShowDiagnosisResult: function (result) {
                this.populateDiagnosisResult(result);
                this.toggleDiagnosisModal(true);
            },
            handleOnCreateNewConsultation: function () {
               
            },
        },
        events: {
            initCommonEvents: function () {
                registerEvent(
                    elementHolders.common.buttons.newConsultation,
                    "click",
                    function (event) {
                        window.location.href = "AdminDiagnosis?tabName=Consultation";
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
        },
        apiService: {
            getDiagnosis: async function () {
                return await apiFetch(
                    URLS.getDiagnosis,
                    {
                        params: stateHolders.searchParams
                    });
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