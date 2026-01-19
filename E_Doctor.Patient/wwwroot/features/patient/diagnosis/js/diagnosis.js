(function () {
    const DIAGNOSIS_BASE_URL = "Diagnosis";
    const URLS = {
        getSymptoms: `${DIAGNOSIS_BASE_URL}/GetSymptoms`,
        runDiagnosis: `${DIAGNOSIS_BASE_URL}/RunDiagnosis`,
        getDiagnosis: `${DIAGNOSIS_BASE_URL}/GetDiagnosis`,
        getDiagnosisById: `${DIAGNOSIS_BASE_URL}/GetDiagnosisById`,
        deleteDiagnosisById: `${DIAGNOSIS_BASE_URL}/DeleteDiagnosisById`,
        importRulesConfiguration: `${DIAGNOSIS_BASE_URL}/ImportRulesConfiguration`,
        getPhysicalExamItems: `${DIAGNOSIS_BASE_URL}/GetPhysicalExamItems`,
        savePhysicalExamReport: `${DIAGNOSIS_BASE_URL}/SavePhysicalExamReport`,
        getPhysicalExamById: `${DIAGNOSIS_BASE_URL}/GetPhysicalExamById`,
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
            physicalReport: {
                root: "#physicalReportModal",
                tableBody: "#pyhsicalReportTableBody",
                patientName: "#patientPhysicalReportName",
                vitalSignsFields: ".physical-exam-vital-sign-field",
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
                },
                savePhysicalExamBtn: "#savePhysicalExam",
                closePhysicalExamModalBtn: "#closePhysicalReportBtn"
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
            services.events.initCommonEvents();
            services.events.initDiagnosis();
            services.events.initImportRulesevent();
            services.events.initPhysicalExam();
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
                    //const $tr = $(`
                    //    <tr>
                    //        <td>${convertDateTimeToLocal(item.diagnoseDate)}</td>
                    //        <td style="white-space: normal;">${item.symptoms}</td>
                    //        <td>${item.illnessName}</td>
                    //        <td><a href="#" class="btn-view-diagnosis">View Diagnosis</a></td>
                    //    </tr>
                    //`);

                    const $tr = $(`
                        <tr>
                            <td>${convertDateTimeToLocal(item.diagnoseDate)}</td>
                            <td style="white-space: normal;">${item.symptoms}</td>
                            <td>${item.illnessName}</td>
                            <td>
                                ${item.physicalExamId > 0 ? createPhysicalExamBtn() : ""}
                                <button class="btn btn-primary btn-view-diagnosis" style="padding: 0.5rem;" title="View Consultation Pre-Result">
                                    <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                                </button>
                                <button class="btn btn-danger btn-delete-diagnosis" style="padding: 0.5rem;" title="Delete Diagnosis">
                                    <svg style="width:1rem; height:1rem;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
                                </button>
                             </td>
                            <td><span class="status-badge ${item.physicalExamId == 0 ? 'status-pending' : 'status-completed'}">${item.physicalExamId == 0 ? "Pending" : "Completed"}</span></td>
                        </tr>
                    `);

                    function createPhysicalExamBtn() {
                        return `<button class="btn btn-secondary btn-view-physical-exam" style="padding: 0.5rem;" title="Physical Examination">
                                    <svg style="width:1rem; height:1rem;" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path></svg>
                                </button>`
                    }

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
                            services.eventHandlers.handleOnClickPhysicalExam(item);
                        }
                    );

                    // Print Receipt
                    //registerEvent(
                    //    $tr.find(".btn-print-receipt"),
                    //    "click",
                    //    async function (event) {
                    //        // Assume jsPDF is loaded globally
                    //        const { jsPDF } = window.jspdf;

                    //        // --- CONFIGURATION ---
                    //        const imagePath = "/print_receipt.png";
                    //        const pdfFilename = "Print Receipt.pdf";
                    //        // ---------------------

                    //        /**
                    //         * Creates the jsPDF document and adds the loaded Image object, filling the entire page.
                    //         * * @param {HTMLImageElement} img - The fully loaded Image object.
                    //         * @param {string} filename - The desired name for the PDF file.
                    //         */
                    //        function createPDF(img, filename) {
                    //            // Initialize jsPDF document (Portrait, millimeters, A4 size is default)
                    //            const doc = new jsPDF({
                    //                orientation: 'portrait',
                    //                unit: 'mm',
                    //                format: 'a4'
                    //            });

                    //            if (!(img instanceof HTMLImageElement)) {
                    //                console.error("PDF creation failed: Image object is invalid or not fully loaded.");
                    //                return;
                    //            }

                    //            // --- KEY CHANGE: FORCING IMAGE TO FILL PAGE ---
                    //            const pageHeight = doc.internal.pageSize.getHeight();
                    //            const pageWidth = doc.internal.pageSize.getWidth();

                    //            // The image's new dimensions are set directly to the page's dimensions.
                    //            // This will stretch or squash the image to fit the entire page.
                    //            const newWidth = pageWidth;
                    //            const newHeight = pageHeight;

                    //            // Since the image fills the page, it starts at the top-left corner (0, 0).
                    //            const x = 0;
                    //            const y = 0;
                    //            // --- END KEY CHANGE ---

                    //            // Add the image to the PDF
                    //            // The image is added with no margin and fills the page completely.
                    //            doc.addImage(img, 'PNG', x, y, newWidth, newHeight);

                    //            // Save the PDF and trigger the client-side download
                    //            doc.save(filename);
                    //            console.log(`PDF successfully generated and saved as: ${filename}`);
                    //        }


                    //        /**
                    //         * Function to start the PDF generation process from a hosted image path.
                    //         * Loads the image asynchronously, and calls createPDF upon success.
                    //         */
                    //        function generatePDFFromHostedImage(imagePath, pdfFilename) {
                    //            const img = new Image();
                    //            img.crossOrigin = '';

                    //            img.onload = function () {
                    //                createPDF(img, pdfFilename);
                    //            };

                    //            img.onerror = function () {
                    //                console.error(`ERROR: Failed to load image from: ${imagePath}. Check the path.`);
                    //            };

                    //            img.src = imagePath;
                    //        }

                    //        // --- EXECUTION ---
                    //        // Call the main function that handles image loading and then PDF creation
                    //        generatePDFFromHostedImage(imagePath, pdfFilename);
                    //    }
                    //);
                    $diagnosisTblBody.append($tr);
                });

                stateHolders.diagnosisPaginatedResult.totalPages = result.totalPages;

                this.createPaginations();
            },
            handleOnClickPhysicalExam: function (diagnosis) {
                const { physicalReport: physicalReportElmts } = elementHolders.modals;

                const { physicalExamId, displayName } = diagnosis;

                $(physicalReportElmts.patientName).text(displayName);
                this.createPhysicalExamTable();

                if (physicalExamId != 0) {
                    this.handleOnGetPhysicalExam(physicalExamId);
                    $(physicalReportElmts.root).addClass("visible");
                }
            },
            handleOnClosePhysicalExamModal: function () {
                const { physicalReport: physicalReportElmts } = elementHolders.modals;
                $(physicalReportElmts.root).removeClass("visible");
                $(physicalReportElmts.vitalSignsFields).val("");
            },
            handleOnGetPhysicalExam: async function (physicalExamId) {
                const { physicalReport: physicalReportElmts } = elementHolders.modals;
                const result = await services.apiService.getPhysicalExamById(physicalExamId);

                $(physicalReportElmts.headerFields.bp).val(result.bp ?? "");
                $(physicalReportElmts.headerFields.hr).val(result.hr ?? "");
                $(physicalReportElmts.headerFields.o2Sat).val(result.o2Sat ?? "");
                $(physicalReportElmts.headerFields.rr).val(result.rr ?? "");
                $(physicalReportElmts.headerFields.temp).val(result.temp ?? "");
                $(physicalReportElmts.headerFields.weight).val(result.weight ?? "");

                if (result.physicalExamFindings != null && result.physicalExamFindings.length > 0) {
                    result.physicalExamFindings.map(item => {
                        const { physicalItemId, isNormal, normalDescription, abnormalFindings } = item;
                        const $isNormalElmt = $(`#physical-item-isnormal-${physicalItemId}`);

                        if ($isNormalElmt.attr("type") == "checkbox") {
                            $isNormalElmt.prop("checked", isNormal);
                        } else {
                            $isNormalElmt.val(normalDescription ?? "");
                        }

                        const $abnormalFindingsElmt = $(`#physical-item-abnormalfindings-${physicalItemId}`);
                        $abnormalFindingsElmt.val(abnormalFindings ?? "");
                    });
                }
            },
            createPhysicalExamTable: function () {
                const { physicalReport: physicalReportElmts } = elementHolders.modals;
                const $tblBody = $(physicalReportElmts.tableBody);
                $tblBody.empty();
                stateHolders.physicalExamItems.forEach(item => {
                    const $tr = $("<tr>");

                    const $descDataCol = $(`<td class="tbl-data desc-col">`).html(item.label);
                    const $isNormalDataCol = $(`<td class="tbl-data normal-col">`);
                    const $abNormalFindingsDataCol = $(`<td class="tbl-data abnormal-col">`);

                    if (item.label.toLowerCase() == "others") {
                        const $isNormalTxtField = $(`<input id="physical-item-isnormal-${item.physicalItemId}" placeholder="${item.label} Normal Findings"  type="text" class="form-input">`)
                        $isNormalDataCol.append($isNormalTxtField);
                    }
                    else {
                        const $isNormalCkbx = $(`
                                <div class="custom-checkbox">
                                    <input id="physical-item-isnormal-${item.physicalItemId}" type="checkbox" class="custom-checkbox-input">
                                    <label for="physical-item-isnormal-${item.physicalItemId}" class="custom-checkbox-label"></label>
                                </div>
                        `);

                        $isNormalDataCol.append($isNormalCkbx);
                    }

                    const $abnormalFindingsField = $(`<textarea id="physical-item-abnormalfindings-${item.physicalItemId}" class="form-textarea"></textarea>`);
                    $abNormalFindingsDataCol.append($abnormalFindingsField);

                    $tr.append($descDataCol);
                    $tr.append($isNormalDataCol);
                    $tr.append($abNormalFindingsDataCol);

                    $tblBody.append($tr);
                });
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
            populateDiagnosisResult: function ({ result, description, symptoms = [] }) {
                const { diagnosis } = elementHolders.modals;

                const $illnessSection = $("#potential-illness-container");
                $illnessSection.empty();

                if (result.length > 0) {
                    result.map(item => {
                        const $card = potentialMatchesCard(item.illness, item.description);
                        $illnessSection.append($card);
                    });
                } else {
                    const $card = potentialMatchesCard(item.notes, "");
                    $illnessSection.append($card);
                }

                const $symptomsContainer = $(".patient-symptoms-list");
                $symptomsContainer.empty();

                symptoms.map(s => {
                    const $item = $(`
                            <li class="patient-symptom-tag">${s}</li>
                        `);

                    $symptomsContainer.append($item);
                });

                function potentialMatchesCard(title, desc) {
                    const $card = $(`<div class="diagnosis-result-card"></div>`);

                    const $resultTxt = $(`<h3>${title}</h3>`);
                    $card.append($resultTxt);

                    if (desc) {
                        const $desc = $(`<span class="diagnosis-result-desc">${desc}</span>`);
                        $card.append($desc);
                    }
                    return $card;
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
                        window.location.href = "/Dashboard";
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

                //registerEvent(
                //    common.buttons.showImportFileModal,
                //    "click",
                //    function (event) {
                //        services.eventHandlers.toggleImportFileModal(true);
                //    }
                //);

                registerEvent(
                    importRule.buttons.close,
                    "click",
                    function (event) {
                        services.eventHandlers.toggleImportFileModal(false);
                    }
                );
            },
            initPhysicalExam: function () {
                registerEvent(
                    elementHolders.modals.physicalReport.closePhysicalExamModalBtn,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnClosePhysicalExamModal();
                    }
                );
            }
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
            getPhysicalExamById: async function (physicalExamId) {
                return await apiFetch(
                    URLS.getPhysicalExamById,
                    {
                        params: {
                            physicalExamId: physicalExamId
                        },
                    });
            },
        }
    };
    document.addEventListener("DOMContentLoaded", services.initialize);
})();