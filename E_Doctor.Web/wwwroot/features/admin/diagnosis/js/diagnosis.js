const SYMPTOMS = [
    { id: 1, name: 'Fever' }, { id: 2, name: 'Cough' }, { id: 3, name: 'Sore Throat' },
    { id: 4, name: 'Headache' }, { id: 5, name: 'Body Aches' }, { id: 6, name: 'Rash' },
    { id: 7, name: 'Itchiness' }, { id: 8, name: 'Fatigue' }, { id: 9, name: 'Difficulty Breathing' },
    { id: 10, name: 'Nausea' }, { id: 11, name: 'Vomiting' }
];

(function () {
    const SYMPTOM_BASE_URL = "";
    const SETTINGS_BASE_URL = "AdminSetting";
    const URLS = {
        getSymptoms: `${SETTINGS_BASE_URL}/GetSymptoms`,

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
                body: ""
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
                    next: "",
                    runDiagnosis: "",
                }
            }
        },
    }
    const services = {
        initialize: function () {
            services.eventHandlers.setSymptoms();

            services.events.initMain();
        },
        eventHandlers: {
            toggleDiseasesModal: function (toOpen, toEdit) {
                const modal = elementHolders.modals.consultation;
                $(modal.root).toggleClass("visible", toOpen);
                $(modal.title).text(toEdit ? "Step 1: Select Symptoms" : "Specify Durations");
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

                const $selectedSymptomList = $(consultation.symptomSelectedItemsParent);
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
            }
        },
        events: {
            initMain: function () {
                const { common } = elementHolders;
                const { consultation } = elementHolders.modals;
                const $symptomSearchParent = $(consultation.symptomSearchParent);
                const symptomSearchParent = document.querySelector(consultation.symptomSearchParent);

                registerEvent(
                    common.buttons.newConsultation,
                    "click",
                    function (event) {
                        services.eventHandlers.toggleDiseasesModal(true, false);
                    }
                );

                registerEvent(
                    consultation.buttons.close,
                    "click",
                    function (event) {
                        services.eventHandlers.toggleDiseasesModal(false);
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

            }
        },
        apiService: {
            getSymptoms: async function () {
                return await apiFetch(URLS.getSymptoms);
            },
        }
    };
    document.addEventListener("DOMContentLoaded", services.initialize);
})();