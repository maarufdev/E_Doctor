(function () {
    const DIAGNOSIS_BASE_URL = "AdminDiagnosis";
    const SETTINGS_BASE_URL = "AdminSetting";
    const URLS = {
        getSymptoms: `${SETTINGS_BASE_URL}/GetIllnessSymptoms`,
        runDiagnosis: `${DIAGNOSIS_BASE_URL}/RunDiagnosis`,
        getDiagnosis: `${DIAGNOSIS_BASE_URL}/GetDiagnosis`
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
                    close: "#close-consultation-modal",
                    next: "#next-step-btn",
                    review: "#review-diagnosis-btn",
                    runDiagnosis: "#run-diagnosis-btn",
                    finish: "#finish-diagnosis-btn",
                },
                modalContents: {
                    symptoms: ".select-symptoms-content",
                    durations: ".selected-symptoms-durations"
                },
                questions: {
                    quesContainer: "#patient-question-container",
                    quesSymptomId: "#quest-symptom-id",
                    quesSymptomDuration: "#quest-symptom-days",
                    quesSymptomRemoveBtn: "#quest-symptom-remove-btn",
                    questionSymptomNameHolder: "#question-symptom-name-holder",
                },
            }
        },
    }
    const services = {
        initialize: function () {
            services.eventHandlers.setSymptoms();
            services.eventHandlers.renderDiagnosisTable();
            services.events.initMain();
        },
        eventHandlers: {
            toggleDiagnosisModal: function (toOpen) {
                const modal = elementHolders.modals.consultation;
                $(modal.root).toggleClass("visible", toOpen);
                $(modal.title).text("Step 1: Select Symptoms");
            },
            renderDiagnosisTable: async function () {
                const { diagnosis } = elementHolders.tables;
                const $diagnosisTblBody = $(diagnosis.body);
                $diagnosisTblBody.empty();

                const diagnosisReponse = await services.apiService.getDiagnosis() ?? [];

                diagnosisReponse.forEach(item => {
                    const $tr = $(`
                        <tr>
                            <td>${item.diagnoseDate}</td>
                            <td style="white-space: normal;">${item.symptoms}</td>
                            <td>${item.illnessName}</td>
                        </tr>
                    `);

                    $diagnosisTblBody.append($tr);
                });
            },
            populateDiagnosisResult: function (result) {
                const $resultList = $("#diagnosis-result-list");
                $resultList.empty();

                result.forEach(item => {
                    const $diagnosItem = $(`<li><span class="font-semibold">${item.illness}:</span> ${item.score}% Match</li>`);
                    $resultList.append($diagnosItem);
                })
            },
            handleOnRunDiagnosis: async function () {
                const command = this.getSymptomDurationData() ?? [];

                if (command.length == 0) {
                    alert("Invalid data.")
                    return;
                }

                const result = await services.apiService.runDiagnosis(command);

                if (!result) return;

                this.populateDiagnosisResult(result);
                this.renderDiagnosisTable();

                $(".consultation-content").addClass("hidden");
                $(".diagnosis-result-content").removeClass("hidden");
                $(".diagnosis-btn").addClass("hidden");
                $("#finish-diagnosis-btn").removeClass("hidden");
                $("#consultation-modal-title").text("Diagnosis Result");
            },
            getSymptomDurationData: function () {

                //const command = [...document.querySelectorAll(".symptom-duration-input")]
                //    .map(item => {
                //        return {
                //            symptomId: parseInt(item.dataset.symptomId),
                //            duration: parseInt(item.value)
                //        }
                //    });
                //return command
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

                //if (stateHolders.selectedSymptoms.length == 0) {
                //    alert("Please select at least one symptom.");
                //    return;
                //}

                //$(consultation.modalContents.symptoms).addClass("hidden");
                //$(consultation.modalContents.durations).removeClass("hidden");

                //$(".diagnosis-btn").addClass("hidden");
                //$(consultation.buttons.runDiagnosis).removeClass("hidden");

                //$(consultation.title).text("Step 2: Specify Duration");

                //this.handleOnCreateSymptomsDurations();

                const symptomId = parseInt($(consultation.questions.quesSymptomId).val());
                const duration = parseInt($(consultation.questions.quesSymptomDuration).val());

                $(consultation.questions.quesSymptomRemoveBtn).data("symptomId", 0);
                const symptomName = $(consultation.questions.questionSymptomNameHolder).text();

                if (symptomId <= 0) {
                    alert("Please select symptom.");
                    return;
                }
                if (duration <= 0) {
                    alert("Please please add symptom duration.");
                    return;
                };

                stateHolders.selectedSymptoms.push({ symptomId, duration, symptomName });
                services.eventHandlers.pupulateSearchSymptomList();
                services.eventHandlers.updateReviewButton();
                $(consultation.questions.quesContainer).addClass("hidden");
                $(consultation.questions.quesSymptomId).val(0);
                $(consultation.questions.quesSymptomDuration).val(1);
                $(consultation.questions.quesSymptomRemoveBtn).data("symptomId", 0);
                $(consultation.questions.questionSymptomNameHolder).text("");
                $(consultation.buttons.next).prop("disabled", true);
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

                const $selectedSymptomList = $(".symptom-tags");
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
                            eventHandlers.populateSymptomQuestion(symptom);
                        }
                    )
                });
            },
            handleOnSymptomsReview: function () {
                const { consultation } = elementHolders.modals;

                if (stateHolders.selectedSymptoms.length == 0) {
                    alert("Please select at least one symptom.");
                    return;
                }

                $(consultation.modalContents.symptoms).addClass("hidden");
                $(consultation.modalContents.durations).removeClass("hidden");

                $(".diagnosis-btn").addClass("hidden");
                $(consultation.buttons.runDiagnosis).removeClass("hidden");

                $(consultation.title).text("Step 2: Review Symptoms");

                this.handleOnCreateSymptomsDurations();
            },
            populateSymptomQuestion: function ({ symptomName, symptomId }) {
                const { consultation } = elementHolders.modals;
                $(consultation.questions.quesContainer).removeClass("hidden");
                $(consultation.questions.quesSymptomId).val(symptomId);
                $(consultation.questions.quesSymptomDuration).val(1);
                $(consultation.questions.quesSymptomRemoveBtn).data("symptomId", symptomId);
                $(consultation.questions.questionSymptomNameHolder).text(symptomName);
                $(consultation.buttons.next).prop("disabled", false);
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
            updateReviewButton: function () {
                const { consultation } = elementHolders.modals;
                $(consultation.buttons.review).prop("disabled", !(stateHolders.selectedSymptoms.length > 0));
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
                $(".symptom-tags").empty();
            },
            handleOnCloseDiagnosisModal: function () {
                this.toggleDiagnosisModal(false);
                $(".consultation-content").addClass("hidden");
                $(".select-symptoms-content").removeClass("hidden");
                $(".diagnosis-btn").addClass("hidden");
                $("#next-step-btn").removeClass("hidden").prop("disabled", true);
                $("#review-diagnosis-btn").removeClass("hidden").prop("disabled", true);

                this.clearConsultations();
                this.clearSymptomQuestion();
            }
        },
        events: {
            initMain: function () {
                const { common } = elementHolders;
                const { consultation } = elementHolders.modals;
                const symptomSearchParent = document.querySelector(consultation.symptomSearchParent);


                registerEvent(
                    common.buttons.newConsultation,
                    "click",
                    function (event) {
                        services.eventHandlers.toggleDiagnosisModal(true);
                        services.eventHandlers.pupulateSearchSymptomList();
                    }
                );

                registerEvent(
                    consultation.buttons.close,
                    "click",
                    function (event) {
                        const isDurationOpen = !$(consultation.modalContents.durations).hasClass("hidden");

                        if (isDurationOpen) {
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
                    consultation.questions.quesSymptomRemoveBtn,
                    "click",
                    function (event) {
                        const dataset = $(event.target).data();
                        services.eventHandlers.handleOnRemoveQuestionSymptom(dataset);
                    }
                );

                registerEvent(
                    consultation.questions.quesSymptomDuration,
                    "input",
                    function (event) {
                        const target = event.target;
                        const currentVal = target.value;

                        if (parseInt(currentVal) <= 0) {
                            target.value = 1;
                        }
                    }
                );
                registerEvent(
                    consultation.buttons.review,
                    "click",
                    function (event) {
                        services.eventHandlers.handleOnSymptomsReview();
                    }
                );
            }
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
            }
        }
    };
    document.addEventListener("DOMContentLoaded", services.initialize);
})();