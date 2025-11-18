(function () {
    const DIAGNOSIS_BASE_URL = "AdminDiagnosis";
    const SETTINGS_BASE_URL = "AdminSetting";
    const URLS = {
        getSymptoms: `${SETTINGS_BASE_URL}/GetIllnessSymptoms`,
        runDiagnosis: `${DIAGNOSIS_BASE_URL}/RunDiagnosis`,
        getDiagnosis: `${DIAGNOSIS_BASE_URL}/GetDiagnosis`,
        getDiagnosisById: `${DIAGNOSIS_BASE_URL}/GetDiagnosisById`,
        deleteDiagnosisById: `${DIAGNOSIS_BASE_URL}/DeleteDiagnosisById`,
        getPhysicalExamItems: `${DIAGNOSIS_BASE_URL}/GetPhysicalExamItems`,
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
        physicalExamItems: [],
    }
    const elementHolders = {
        common: {
            buttons: {
                newConsultation: "#new-consultation-btn",
            },
            diagnosisPagination: "#diagnosis-pagination",
            closeModalBtn: ".modal-close-btn",
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
            },
            physicalReport: {
                root: "#physicalReportModal",
                tableBody: "#pyhsicalReportTableBody",
                patientName: "#patientPhysicalReportName",
                headerFields: {
                    bp: "#patientBP",
                    hr: "#patientHR",
                    rr: "#patientRR",
                    temp: "#patientTemp",
                    weight: "#patientWeight",
                    o2Sat: "#patientO2Sat",
                },
                checklistFields: {
                    normalFields: "",
                    abnormalFindingsFields: ""
                }
            },
        },
    }

    async function setPhysicalExamState() {
        const result = await services.apiService.getPhysicalExamItems();

        stateHolders.physicalExamItems = result ?? [];
    }

    const services = {
        initialize: function () {
            setPhysicalExamState();

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
                    //const $tr = $(`
                    //    <tr>
                    //        <td>${convertDateTimeToLocal(item.diagnoseDate)}</td>
                    //        <td style="white-space: normal;">${item.displayName}</td>
                    //        <td style="white-space: normal;">${item.symptoms}</td>
                    //        <td>${item.illnessName}</td>
                    //        <td>
                    //        <button class="btn btn-primary btn-view-diagnosis" style="padding: 0.5rem;" title="View Diagnosis">
                    //                <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                    //        </button>
                    //            <a href="#" class="btn-view-diagnosis">View Diagnosis</a>
                    //            <a href="#" class="btn-delete-diagnosis">Delete Diagnosis</a>
                    //        </td>
                    //    </tr>
                    //`);

                    const $tr = $(`
                        <tr style="--tw-text-opacity:1;">
                            <td>${convertDateTimeToLocal(item.diagnoseDate)}</td>
                            <td style="white-space: normal;">${item.displayName}</td>
                            <td style="white-space: normal;">${item.symptoms}</td>
                            <td>${item.illnessName}</td>
                            <td>
                                <button class="btn btn-secondary btn-view-physical-exam" style="padding: 0.5rem;" title="Physical Examination">
                                    <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path></svg>
                                </button>
                                <button class="btn btn-warning btn-print-receipt" style="padding: 0.5rem;" title="Print Reciept">
                                    <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m0 0h10M5 11h14M5 17a2 2 0 01-2-2v-2a2 2 0 012-2h14a2 2 0 012 2v2a2 2 0 01-2 2M5 17v4h14v-4"></path></svg>
                                </button>
                                <button class="btn btn-primary btn-view-diagnosis" style="padding: 0.5rem;" title="View Consultation Pre-Result">
                                        <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                                </button>
                                <button class="btn btn-danger btn-delete-diagnosis" style="padding: 0.5rem;" title="Delete Diagnosis">
                                    <svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                                </button>
                            </td>
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
                    registerEvent(
                        $tr.find(".btn-delete-diagnosis"),
                        "click",
                        async function (event) {
                            event.preventDefault();

                            const isYes = confirm("Are you sure to delete this Diagnosis?");

                            if (isYes) {
                                if (diagnosisId) {
                                    const result = await services.apiService.deleteDiagnosisById(diagnosisId);
                                    stateHolders.pageNumber = 1;
                                    await services.eventHandlers.renderDiagnosisTable(result);
                                }
                            }
                        }
                    );

                    // Physical exam report
                    registerEvent(
                        $tr.find(".btn-view-physical-exam"),
                        "click",
                        async function (event) {
                            services.eventHandlers.handleOnClickPhysicalExam(diagnosisId);

                        }
                    );
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
            handleOnClickPhysicalExam: function (diagnosisId) {
                $(elementHolders.modals.physicalReport.root).addClass("visible");
            }

        },
        events: {
            initCommonEvents: function () {
                registerEvent(
                    elementHolders.common.buttons.newConsultation,
                    "click",
                    function (event) {
                        window.location.href = "/";
                    }
                );

                registerEvent(
                    elementHolders.common.closeModalBtn,
                    "click",
                    function (event) {
                        const targetModal = $(event.target).attr("data-target-modal");
                        if (targetModal == "physicalReportModal") {
                            $(`#${targetModal}`).removeClass("visible");
                        }
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
            deleteDiagnosisById: async function (diagnosisId) {
                return await apiFetch(
                    URLS.deleteDiagnosisById,
                    {
                        params: {
                            diagnosisId: diagnosisId
                        },
                    });
            },
            getPhysicalExamItems: async function () {
                return await apiFetch(URLS.getPhysicalExamItems);
            },
        }
    };
    document.addEventListener("DOMContentLoaded", services.initialize);
})();