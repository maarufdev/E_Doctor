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
    }
    const elementHolders = {
        common: {
            buttons: {
                newConsultation: "#new-consultation-btn",
            }
        },
        tables: {
            diagnosis: {
                root: "",
                body: "#history-table-body"
            }
        },
        modals: {
            consultation: {
                root: "#consultation-modal",
                title: "#consultation-modal-title",
                symptomSearchParent: "#symptom-search-parent",
                symptomSearch: "#symptom-search-input",
                symptomSearchList: "#symptom-search-list",
                symptomSearchItem: ".searchable-dropdown-item",
                symptomSelectedItemsParent: "#selected-symptoms-container",
                buttons: {
                    commonButtonContainer: ".consultation-buttons",
                    symptomsContainer: ".symptoms-selection-buttons",
                    symptomsReviewContainer: ".symptoms-review-buttons",
                    symptomsResultContainer: ".symptoms-result-buttons",
                    close: "#close-consultation-modal",
                    next: "#next-step-btn",
                    back: "#back-diagnosis-btn",
                    runDiagnosis: "#run-diagnosis-btn",
                    finish: "#finish-diagnosis-btn",
                },
                modalContents: {
                    common: ".consultation-content",
                    symptoms: ".select-symptoms-content",
                    durations: ".selected-symptoms-durations",
                    result: ".diagnosis-result-content",
                },
                diagnosisResult: {
                    result: "#diagnosis-result",
                    prescription: "#diagnosis-prescriptions"
                },
                questions: {
                    quesContainer: "#patient-question-container",
                    quesSymptomId: "#quest-symptom-id",
                    quesSymptomDuration: "#quest-symptom-days",
                    quesSymptomRemoveBtn: "#quest-symptom-remove-btn",
                    questionSymptomNameHolder: "#question-symptom-name-holder",
                },
            },
            diagnosis: {
                root: "#diagnosis-result-modal",
                buttons: {
                    close: "#close-diagnosis-result-modal"
                },
                details: {
                    result: "#diagnosis-result",
                    description: "#diagnosis-description",
                    prescription: "#diagnosis-prescriptions",
                }
            }
        },
    }
    const services = {
        initialize: function () {
            services.eventHandlers.setSymptoms();
            services.eventHandlers.renderDiagnosisTable();
            services.events.initConsultation();
            services.events.initDiagnosis();
        },
        eventHandlers: {
            toggleConsultationModal: function (toOpen) {
                const modal = elementHolders.modals.consultation;
                $(modal.root).toggleClass("visible", toOpen);
                $(modal.title).text("Step 1: Select Symptoms");
            },

            toggleDiagnosisModal: function (toOpen) {
                const modal = elementHolders.modals.diagnosis;
                $(modal.root).toggleClass("visible", toOpen);
            },
            renderDiagnosisTable: async function () {
                const { diagnosis } = elementHolders.tables;
                const $diagnosisTblBody = $(diagnosis.body);
                $diagnosisTblBody.empty();

                const diagnosisReponse = await services.apiService.getDiagnosis() ?? [];

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
            },
            populateDiagnosisResult: function ({ result, description, prescription }) {
                const { diagnosis } = elementHolders.modals;

                $(diagnosis.details.result).text(" ");
                $(diagnosis.details.prescription).text(" ");
                $(diagnosis.details.description).text(" ");

                if (result) {
                    $(diagnosis.details.result).text(result ?? " ");
                    $(diagnosis.details.prescription).text(prescription ?? " ");
                    $(diagnosis.details.description).text(description ?? " ");
                } 
            },
            handleOnShowDiagnosisResult: function (result) {
                this.populateDiagnosisResult(result);
                this.toggleDiagnosisModal(true);
            },
            handleOnRunDiagnosis: async function () {
                const command = this.getSymptomDurationData() ?? [];

                if (command.length == 0) {
                    alert("Invalid data.")
                    return;
                }

                const result = await services.apiService.runDiagnosis(command);

                if (!result) return;

                this.renderDiagnosisTable();
                this.handleOnShowDiagnosisResult(result);

                $(elementHolders.modals.consultation.modalContents.common).addClass("hidden");
                $(elementHolders.modals.consultation.modalContents.symptoms).removeClass("hidden");
                $(elementHolders.modals.consultation.buttons.commonButtonContainer).addClass("hidden");

                this.toggleConsultationModal(false);
                $(elementHolders.modals.consultation.symptomSelectedItemsParent).empty();
                $(elementHolders.modals.consultation.buttons.next).prop("disabled", true);
                services.eventHandlers.pupulateSearchSymptomList();
                stateHolders.symptomSearchList = [];
                stateHolders.selectedSymptoms = [];
            },
            getSymptomDurationData: function () {
                const command = [...stateHolders.selectedSymptoms]
                    .map(item => {
                        return {
                            symptomId: parseInt(item.symptomId),
                            duration: parseInt(item.duration)
                        }
                    });

                return command
            },

            handleOnCreateSymptomsDurations: function () {
                const { consultation } = elementHolders.modals;
                const $durationContainer = $("#duration-fields");

                $durationContainer.empty();

                stateHolders.selectedSymptoms.forEach((s, idx) => {
                    const duration = $(`
                    <div class="flex items-center justify-between">
                        <label class="font-semibold">${idx + 1}. ${s.symptomName}</label>
                        <input type="text" min="1" value="${s.duration} Days" disabled class="form-input symptom-duration-input" style="width: 8rem;" data-symptom-id="${s.symptomId}">
                    </div>
                    `);
                    $durationContainer.append(duration);
                });
            },

            handleOnClickNext: function () {
                const { consultation } = elementHolders.modals;

                $(consultation.buttons.symptomsContainer).addClass("hidden");
                $(consultation.buttons.symptomsReviewContainer).removeClass("hidden");
                this.handleOnSymptomsReview();
            },
            setSymptoms: async function () {
                const symptoms = await services.apiService.getSymptoms();
                stateHolders.symptoms = symptoms;
                this.pupulateSearchSymptomList();
            },
            pupulateSearchSymptomList: function () {
                const { consultation } = elementHolders.modals;
                const { symptoms } = stateHolders;
                const { eventHandlers } = services;

                const $symptomSearchList = $(consultation.symptomSearchList);
                $symptomSearchList.empty();

                const selectedSymptomIds = stateHolders.selectedSymptoms.map(x => x.symptomId);
                const filteredSymptoms = symptoms.filter(s => !selectedSymptomIds.includes(s.symptomId));

                filteredSymptoms.forEach(symptom => {
                    const { symptomId, symptomName } = symptom;

                    const symptomItemElmt = document.createElement("div");
                    symptomItemElmt.className = "searchable-dropdown-item";
                    symptomItemElmt.dataset.symptomId = symptomId;
                    symptomItemElmt.textContent = symptomName;
                    symptomItemElmt.setAttribute("tabindex", "0");

                    $symptomSearchList.append(symptomItemElmt);

                    registerEvent(
                        symptomItemElmt,
                        "click",
                        function () {
                            eventHandlers.toggleSearchSymptomList(false);
                            eventHandlers.handleOnSymptomSelected(symptom);
                        }
                    );
                });
            },
            handleOnClickBack: function () {
                const { consultation } = elementHolders.modals;
                $(consultation.modalContents.common).addClass("hidden");
                $(consultation.modalContents.symptoms).removeClass("hidden");
                $(consultation.title).text("Step 1: Select Symptoms");

                $(".symptoms-selection-buttons").removeClass("hidden");
                $(".symptoms-review-buttons").addClass("hidden");
                $(".symptoms-result-buttons").addClass("hidden");
            },
            handleOnSymptomsReview: function () {
                const { consultation } = elementHolders.modals;

                if (stateHolders.selectedSymptoms.length == 0) {
                    alert("Please select at least one symptom.");
                    return;
                }

                $(consultation.modalContents.symptoms).addClass("hidden");
                $(consultation.modalContents.durations).removeClass("hidden");
                $(consultation.buttons.runDiagnosis).removeClass("hidden");
                $(consultation.title).text("Step 2: Review Symptoms");

                this.handleOnCreateSymptomsDurations();
            },
            handleOnSymptomSelected: function ({ symptomName, symptomId }) {
                const { consultation } = elementHolders.modals;
                
                const $nextButton = $(consultation.buttons.next)
                const $questionElmt = $(`
                    <div class="patient-symptom">
                        <p>How long have you had <span id="question-symptom-name-holder" class="font-semibold">${symptomName}</span>?</p>
                        <div class="patient-question">
                            <input value="1" type="number" placeholder="Days" class="form-input symptom-duration" />
                            <button class="btn btn-sm btn-danger remove-patient-symptom-btn" data-symptom-id="${symptomId}">x</button>
                        </div>
                    </div>
                `);

                $questionElmt.data("symptom-id", symptomId);
                $questionElmt.find(".symptom-duration").off("input").on("input", function (event) {
                    const target = event.target;
                    const currentVal = parseInt(target.value);
                    if (currentVal <= 0) {
                        target.value = 1;
                    }

                    const symptomIndex = stateHolders.selectedSymptoms.findIndex(s => s.symptomId == symptomId);
                    if (stateHolders.selectedSymptoms[symptomIndex]) {
                        stateHolders.selectedSymptoms[symptomIndex].duration = currentVal;
                    }
                });
                $questionElmt.find(".remove-patient-symptom-btn").off("click").on("click", function (event) {
                    stateHolders.selectedSymptoms = stateHolders.selectedSymptoms.filter(s => s.symptomId != symptomId);
                    services.eventHandlers.pupulateSearchSymptomList();
                    $questionElmt.remove();

                    if (stateHolders.selectedSymptoms.length == 0) {
                        $nextButton.prop("disabled", true);
                    }
                });

                stateHolders.selectedSymptoms.push({ symptomId: parseInt(symptomId), symptomName: symptomName, duration: 1 });
                $(consultation.symptomSelectedItemsParent).prepend($questionElmt);
                services.eventHandlers.pupulateSearchSymptomList();

                $nextButton.prop("disabled", false);

            },
            handleOnRemoveQuestionSymptom: function ({ symptomId }) {
                this.clearSymptomQuestion();
            },
            clearSymptomQuestion: function () {
                const { consultation } = elementHolders.modals;
                $(consultation.questions.quesContainer).addClass("hidden");
                $(consultation.questions.quesSymptomId).val(0);
                $(consultation.questions.quesSymptomDuration).val(1);
                $(consultation.questions.quesSymptomRemoveBtn).data("symptomId", 0);
                $(consultation.questions.questionSymptomNameHolder).text("");
            },
            createSymptomTag: function (symptom) {
                const { symptomId, symptomName } = symptom;
                const pupulateSearchSymptomList = this.pupulateSearchSymptomList;
                const tagParentElmt = document.createElement("div");
                tagParentElmt.className = "symptom-tag";

                const symtomNameElmt = document.createElement("span");
                symtomNameElmt.textContent = symptomName ?? "";

                const tagCloseBtn = document.createElement("button");
                tagCloseBtn.dataset.id = symptomId;
                tagCloseBtn.innerHTML = "&times;"

                tagCloseBtn.addEventListener("click", () => {
                    stateHolders.selectedSymptoms = stateHolders.selectedSymptoms.filter(s => s.symptomId != symptomId);
                    tagParentElmt.remove();
                    pupulateSearchSymptomList();
                });

                tagParentElmt.appendChild(symtomNameElmt);
                tagParentElmt.appendChild(tagCloseBtn);

                return tagParentElmt;      
            },
            toggleSearchSymptomList: function (toShow) {
                const { consultation } = elementHolders.modals;
                const $searchList = $(consultation.symptomSearchList);
                $searchList.toggleClass("visible", toShow);

                if (toShow) {
                    const { consultation } = elementHolders.modals
                    this.handleOnSearchSymptoms($(consultation.symptomSearch).val());
                }
            },
            handleOnSearchSymptomsOnClose: function () {
                this.toggleSearchSymptomList(false);

            },
            handleOnSearchSymptoms: function (text) {
                const searchVal = text ?? "";
                [...$(".searchable-dropdown-item")].forEach(item => {
                    const itemText = item.textContent.toLowerCase();

                    if (itemText.includes(searchVal.toLowerCase())) {
                        item.classList.remove("hidden");
                    } else {
                        item.classList.add("hidden");
                    }
                })
            },
            handleOnDropdownSelect: function ($item) {
                const symptom = {
                    symptomId: $item.data("id"),
                    symptomName: $item.text(),
                }
            },
            clearConsultations: function () {
                const { consultation } = elementHolders.modals;

                stateHolders.selectedSymptoms = [];

                $("#duration-fields").empty();
            },
            handleOnCloseDiagnosisModal: function () {
                this.toggleConsultationModal(false);

                $(elementHolders.modals.consultation.modalContents.common).addClass("hidden");
                $(elementHolders.modals.consultation.modalContents.symptoms).removeClass("hidden");
                $(elementHolders.modals.consultation.buttons.commonButtonContainer).addClass("hidden");

                this.clearConsultations();
                this.clearSymptomQuestion();
            },
            handleOnCreateNewConsultation: function () {
                stateHolders.symptomSearchList = [];
                services.eventHandlers.toggleConsultationModal(true);
                services.eventHandlers.pupulateSearchSymptomList();
                $(elementHolders.modals.consultation.symptomSelectedItemsParent).empty();
                $(elementHolders.modals.consultation.buttons.commonButtonContainer).addClass("hidden");
                $(elementHolders.modals.consultation.buttons.symptomsContainer).removeClass("hidden");
                $(elementHolders.modals.consultation.buttons.next).prop("disabled", true);
            },
        },
        events: {
            initConsultation: function () {
                const { common } = elementHolders;
                const { consultation } = elementHolders.modals;
                const symptomSearchParent = document.querySelector(consultation.symptomSearchParent);


                registerEvent(
                    common.buttons.newConsultation,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnCreateNewConsultation();
                    }
                );

                registerEvent(
                    consultation.buttons.close,
                    "click",
                    function (event) {
                        const isSymptomSelectionOpen = !$(consultation.modalContents.symptoms).hasClass("hidden");
                        const isDurationOpen = !$(consultation.modalContents.durations).hasClass("hidden");
                        const isStillOpen = (isSymptomSelectionOpen || isDurationOpen);

                        if (isStillOpen) {
                            const yes = confirm("Are you sure you don't want to continue with your diagnosis?");

                            if (!yes) return;
                        }

                        services.eventHandlers.handleOnCloseDiagnosisModal();
                    }
                );

                registerEvent(
                    consultation.symptomSearch,
                    "input",
                    function (event) {
                        const text = event.target.value;
                        services.eventHandlers.handleOnSearchSymptoms(text);
                    }
                );

                registerEvent(
                    consultation.symptomSearch,
                    "blur",
                    function (event) {
                        const related = event.relatedTarget;
                        const isDescendants = symptomSearchParent.contains(related);

                        if (related && isDescendants) {
                            return;
                        }

                        services.eventHandlers.handleOnSearchSymptomsOnClose();
                    }
                );
                registerEvent(
                    consultation.symptomSearch,
                    "focus",
                    function (event) {
                        services.eventHandlers.toggleSearchSymptomList(true);
                    }
                );

                registerEvent(
                    consultation.buttons.next,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnClickNext();
                    }
                );

                registerEvent(
                    consultation.buttons.runDiagnosis,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnRunDiagnosis();
                    }
                );

                registerEvent(
                    consultation.buttons.finish,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnCloseDiagnosisModal();
                    }
                );
                registerEvent(
                    consultation.buttons.back,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnClickBack();
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
            },
        },
        apiService: {
            getSymptoms: async function () {
                return await apiFetch(URLS.getSymptoms);
            },
            getDiagnosis: async function () {
                return await apiFetch(URLS.getDiagnosis)
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