const SYMPTOMS = [
    { id: 1, name: 'Fever' }, { id: 2, name: 'Cough' }, { id: 3, name: 'Sore Throat' },
    { id: 4, name: 'Headache' }, { id: 5, name: 'Body Aches' }, { id: 6, name: 'Rash' },
    { id: 7, name: 'Itchiness' }, { id: 8, name: 'Fatigue' }, { id: 9, name: 'Difficulty Breathing' },
    { id: 10, name: 'Nausea' }, { id: 11, name: 'Vomiting' }
];

const CONSULTATION_HISTORY = [
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Common Cold (100%)",
    },
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Flu (100%)",
    },
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Flu (100%)",
    },
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Flu (100%)",
    },
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Flu (100%)",
    },
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Flu (100%)",
    },
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Flu (100%)",
    },
    {
        diagnosisId: 1,
        diagnosisDate: "8/21/2025, 6:56:41 PM",
        diagnosisSymptoms: "Fever (1 days), Cough (1 days), Sore Throat (1 days), Headache (1 days)",
        diagnosisResult: "Flu (100%)",
    },
];

(function () {
    const DIAGNOSIS_BASE_URL = "AdminDiagnosis";
    const SETTINGS_BASE_URL = "AdminSetting";
    const URLS = {
        getSymptoms: `${SETTINGS_BASE_URL}/GetSymptoms`,
        runDiagnosis: `${DIAGNOSIS_BASE_URL}/RunDiagnosis`,
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
                    runDiagnosis: "#run-diagnosis-btn",
                    finish: "#finish-diagnosis-btn",
                },
                modalContents: {
                    symptoms: ".select-symptoms-content",
                    durations: ".selected-symptoms-durations"
                }
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

                CONSULTATION_HISTORY.forEach(item => {
                    const $tr = $(`
                        <tr>
                            <td>${item.diagnosisDate}</td>
                            <td style="white-space: normal;">${item.diagnosisSymptoms}</td>
                            <td>${item.diagnosisResult}</td>
                        </tr>
                    `);

                    $diagnosisTblBody.append($tr);
                });
            },
            handleOnRunDiagnosis: function () {
                const command = this.getSymptomDurationData() ?? [];

                if (command.length == 0) {
                    alert("Invalid data.")
                    return;
                }

                const result = services.apiService.runDiagnosis(command);

                if (!result) return;

                $(".consultation-content").addClass("hidden");
                $(".diagnosis-result-content").removeClass("hidden");
                $(".diagnosis-btn").addClass("hidden");
                $("#finish-diagnosis-btn").removeClass("hidden");
                $("#consultation-modal-title").text("Diagnosis Result");
            },
            getSymptomDurationData: function () {

                const command = [...document.querySelectorAll(".symptom-duration-input")]
                    .map(item => {
                        return {
                            symptomId: parseInt(item.dataset.symptomId),
                            duration: parseInt(item.value)
                        }
                    });
                return command
            },

            handleOnCreateSymptomsDurations: function () {
                const { consultation } = elementHolders.modals;

                const $durationContainer = $("#duration-fields");

                stateHolders.selectedSymptoms.forEach(s => {
                    const duration = $(`
                    <div class="flex items-center justify-between">
                        <label class="font-semibold">${s.symptomName}</label>
                        <input type="number" min="1" value="1" class="form-input symptom-duration-input" style="width: 8rem;" data-symptom-id="${s.symptomId}">
                    </div>
                    `);

                    const $symptomInput = $(duration).find(".symptom-duration-input");
                    $symptomInput.off("input").on("input", function (e) {
                        const val = e.target.value;
                        if (!val || parseInt(val) <= 0) {
                            e.target.value = 1;
                        }
                    });

                    $symptomInput.off("keydown").on("keydown", function (e) {
                        if (['e', 'E', '.', '+', '-'].includes(e.key)) {
                            e.preventDefault();
                        }
                    });

                    $durationContainer.append(duration);
                });
            },

            handleOnClickedNext: function () {
                const { consultation } = elementHolders.modals;

                if (stateHolders.selectedSymptoms.length == 0) {
                    alert("Please select at least one symptom.");
                    return;
                }

                $(consultation.modalContents.symptoms).addClass("hidden");
                $(consultation.modalContents.durations).removeClass("hidden");

                $(".diagnosis-btn").addClass("hidden");
                $(consultation.buttons.runDiagnosis).removeClass("hidden");

                $(consultation.title).text("Step 2: Specify Duration");

                this.handleOnCreateSymptomsDurations();
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
                            $selectedSymptomList.append(eventHandlers.createSymptomTag(symptom));
                            stateHolders.selectedSymptoms.push(symptom);
                            eventHandlers.pupulateSearchSymptomList();
                        }
                    )
                });
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
                $("#next-step-btn").removeClass("hidden");

                this.clearConsultations();
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
                        services.eventHandlers.handleOnClickedNext();
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
            }
        },
        apiService: {
            getSymptoms: async function () {
                return await apiFetch(URLS.getSymptoms);
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