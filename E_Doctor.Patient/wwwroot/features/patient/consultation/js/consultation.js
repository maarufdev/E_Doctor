(function () {
    const SELECTION_TYPES = {
        NO: 0,
        YES: 1,
        NONE: 2
    };
    const DIAGNOSIS_BASE_URL = "Diagnosis";
    const URLS = {
        getSymptomsByIllnessId: `${DIAGNOSIS_BASE_URL}/GetConsultationSymptomByIllnessId`,
        getIllnesses: `${DIAGNOSIS_BASE_URL}/GetConsultationIllnessList`,
        runDiagnosis: `${DIAGNOSIS_BASE_URL}/RunDiagnosis`
    }
    const stateHolders = {
        currentIllnessId: null,
        illnessList: [],
        symptoms: [],
        selectedSymptoms: [],
        currentSymptomIndex: 0,
        symptomModalTitle: ""
    }
    const elementHolders = {
        buttons: {
            startNewConsultation: "#start-consultation-btn",
        },
        modals: {
            illness: {
                root: "#illness-modal",
                buttons: {
                    close: "#close-illness-modal-btn",
                },
                search: "#search-illness-txt",
                illnessList: "#illness-list",
                illnessItem: ".illness-item-btn",
            },
            symptoms: {
                root: "#symptoms-modal",
                title: "#symptoms-modal-title",
                card: ".symptom-card-container",
                questionContent: "#symptom-question-content",
                buttons: {
                    yes: "#quest-yes-btn",
                    no: "#quest-no-btn",
                    next: "#quest-next-btn",
                    back: "#quest-back-btn",
                    getDiagnosis: "#quest-get-diagnosis-btn",
                    yesNo: ".yes-no-button",
                    cancel: "#cancel-diagnosis",
                    close: "#close-symptom-selection-symptom-btn"
                },
                question: {
                    symptom: "#symptom-quest-name",
                }
            },
            diagnosis: {
                root: "#diagnosis-result-modal",
                buttons: {
                    start: "#start-diagnosis-modal-btn",
                    close: "#close-result-modal-btn",
                },
                diagnosisInfo: {
                    result: "#diagnosis-result",
                    description: "#diagnosis-description",
                    prescription: "#diagnosis-prescription",
                    notes: "#diagnosis-notes",
                }
            },
        }
    }
    const services = {
        initialize: function () {
            services.eventHandlers.illness.setIllnessList();
            services.eventHandlers.illness.setCommonSymptoms();
            services.events.initMain();
        },
        eventHandlers: {
            modal: {
                toggleIllness: function (flag = false) {
                    const { modals } = elementHolders;
                    $(modals.illness.root).toggleClass("visible", flag);
                },
                toggleSymptom: function (flag = false) {
                    const { modals } = elementHolders;
                    $(modals.symptoms.root).toggleClass("visible", flag);
                    stateHolders.symptomModalTitle ? $(modals.symptoms.title).text(stateHolders.symptomModalTitle) : null;
                },

                toggleDiagnosisResult: function (flag = false) {
                    const { modals } = elementHolders;
                    $(modals.diagnosis.root).toggleClass("visible", flag);
                },
            },
            illness: {
                setCommonSymptoms: async function () {
                    const symptomResult = await services.apiService.getSymptomsByIllnessId(null) ?? [];

                    stateHolders.symptoms = symptomResult;
                    services.eventHandlers.symptom.populateSymptomQuestion();
                },
                populateIllnessList: function () {
                    const { modals } = elementHolders;
                    const $illnessList = $(modals.illness.illnessList);
                    $illnessList.empty();

                    const result = stateHolders.illnessList;

                    result.forEach(item => {
                        const $illnessItem = $(`
                            <button type="button" class="btn illness-item-btn" title="${item.illnessName}" data-illness-id="${item.illnessId}">${item.illnessName}</button>
                        `);

                        registerEvent(
                            $illnessItem,
                            "click",
                            async function () {
                                const symptomResult = await services.apiService.getSymptomsByIllnessId(item.illnessId) ?? [];

                                stateHolders.currentIllnessId = item.illnessId;
                                stateHolders.symptomModalTitle = item.illnessName;

                                stateHolders.symptoms = symptomResult.map(item => {
                                    const obj = { ...item, active: false, selectType: SELECTION_TYPES.NONE };
                                    return obj;
                                });

                                stateHolders.currentSymptomIndex = 0;

                                setTimeout(() => {
                                    services.eventHandlers.modal.toggleIllness(false);
                                }, 500);

                                services.eventHandlers.modal.toggleSymptom(true);
                                services.eventHandlers.symptom.populateSymptomQuestion();
                            }
                        );
                        $illnessList.append($illnessItem);
                    })
                },
                handleOnSearchIllness: function (searchText = null) {
                    const { modals } = elementHolders;
                    const $illnessList = $(modals.illness.illnessList);
                    searchText = (searchText) ? searchText.toLowerCase() : "";
                    [...$illnessList.find(".illness-item-btn")].forEach(item => {
                        const buttonText = item.textContent.toLowerCase();
                        buttonText.includes(searchText) ? item.classList.remove("hidden") : item.classList.add("hidden");
                    });
                },
                setIllnessList: async function () {
                    stateHolders.illnessList = await services.apiService.getIllnesses() ?? [];
                },
            },
            symptom: {
                elements: elementHolders.modals.symptoms,
                
                populateSymptomQuestion: function () {
                    const { question, buttons, questionContent } = this.elements;
                    const symptoms = stateHolders.symptoms;

                    const $symptomList = $(".symptoms-list");
                    $symptomList.empty();

                    symptoms.map(symptom => {
                        const $symptomCard = $('<li>').addClass("symptom-item");
                        const $customCheckbox = $(`
                         <div class="custom-checkbox">
                                <input type="checkbox"
                                       id="symptom-${symptom.symptomId}"
                                       class="custom-checkbox-input form-checkbox"
                                       data-id=${symptom.symptomId}
                                       />
                                <label for="symptom-${symptom.symptomId}" class="custom-checkbox-label">${symptom.symptomName}</label>
                            </div>
                        `);

                        $symptomCard.append($customCheckbox);
                        $symptomList.append($symptomCard);

                        $customCheckbox.find("input").off("change").on("change", handleOnCheckboxChanged);
                    });

                    function handleOnCheckboxChanged(event) {
                        const { target } = event;

                        const symptomId = parseInt(target.getAttribute("data-id"));
                        const isSelected = $(target).is(":checked");

                        if (isSelected) {
                            const isSelectedAlreadyExist = stateHolders.selectedSymptoms.some(item => item == symptomId);
                            if (isSelectedAlreadyExist) return;

                            stateHolders.selectedSymptoms.push(symptomId);
                        } else {
                            stateHolders.selectedSymptoms = stateHolders.selectedSymptoms.filter(item => item != symptomId);
                        }
                    }
                },
                handleOnCancel: function () {
                    if (confirm("Are you sure to cancel diagnosis?")) {
                        stateHolders.selectedSymptoms = [];
                        services.eventHandlers.modal.toggleSymptom(false);
                        services.eventHandlers.modal.toggleIllness(false);
                    }
                },
                handleOnShowSymptomsSelections: () => {
                    const { eventHandlers } = services;

                    stateHolders.symptomModalTitle = "New Consultation";
                    eventHandlers.modal.toggleSymptom(true);
                    eventHandlers.illness.setCommonSymptoms();
                }
            },
            diagnosis: {
                elements: elementHolders.modals.diagnosis,
                handleOnGetDiagnosis: async function () {
                    const hasSelectedItem = stateHolders.selectedSymptoms.length > 0;

                    if (!hasSelectedItem) {
                        alert("No symptom selected.\nPlease select at least 1 symptom.");
                        return;
                    }

                    const command = {
                        symptomIds: stateHolders.selectedSymptoms
                    }

                    const result = await services.apiService.runDiagnosis(command);

                    if (result) {
                        this.populateDiagnosisResult(result);
                        stateHolders.selectedSymptoms = [];
                    }

                },
                populateDiagnosisResult: function ({ result, description, prescription, notes, isSuccess, symptoms = [] }) {
                    services.eventHandlers.modal.toggleDiagnosisResult(true);

                    const { diagnosisInfo: elmt } = this.elements;
                    $(elmt.result)
                        .text(result ?? "")
                        .toggleClass("warning-text", !isSuccess);
                    $(elmt.description).text(description ?? "");
                    $(elmt.prescription).text(prescription ?? "");
                    $(elmt.notes)
                        .text(notes ?? "")
                        .toggleClass("warning-text", !isSuccess);

                    const $symptomsContainer = $(".patient-symptoms-list");
                    $symptomsContainer.empty();

                    symptoms.map(s => {
                        const $item = $(`
                            <li class="patient-symptom-tag">${s}</li>
                        `);

                        $symptomsContainer.append($item);
                    })

                },
                clearDiagnosisText: function () {
                    const { diagnosisInfo: elmt } = this.elements;
                    $(elmt.result).text("");
                    $(elmt.description).text("");
                    $(elmt.prescription).text("");
                    $(elmt.notes).text("");
                },
                handleOnStartNewDiagnosis: function () {
                    this.clearDiagnosisText();
                    services.eventHandlers.modal.toggleSymptom(false);
                    services.eventHandlers.modal.toggleDiagnosisResult(false);
                },
                
            }
        },
        events: {
            initMain: function () {
                const { buttons, modals } = elementHolders;
                const { eventHandlers } = services;

                registerEvent(
                    buttons.startNewConsultation,
                    "click",
                    function (e) {
                        eventHandlers.symptom.handleOnShowSymptomsSelections();
                    }
                );

                //registerEvent(
                //    modals.illness.buttons.close,
                //    "click",
                //    function (e) {
                //        eventHandlers.modal.toggleIllness(false);
                //        stateHolders.currentIllnessId = null;
                //    }
                //);

                //registerEvent(
                //    modals.illness.search,
                //    "input",
                //    function (e) {
                //        eventHandlers.illness.handleOnSearchIllness(e.target.value);
                //    }
                //);

                // symptoms
                registerEvent(
                    modals.symptoms.buttons.getDiagnosis,
                    "click",
                    function (e) {
                        eventHandlers.diagnosis.handleOnGetDiagnosis();
                    }
                );

                registerEvent(
                    modals.symptoms.buttons.cancel,
                    "click",
                    function (e) {
                        eventHandlers.symptom.handleOnCancel();
                    }
                );

                registerEvent(
                    modals.symptoms.buttons.close,
                    "click",
                    function (e) {
                        eventHandlers.symptom.handleOnCancel();
                    }
                );

                registerEvent(
                    modals.diagnosis.buttons.start,
                    "click",
                    function (e) {
                        eventHandlers.diagnosis.handleOnStartNewDiagnosis();
                    }
                );

                registerEvent(
                    modals.diagnosis.buttons.close,
                    "click",
                    function (e) {
                        eventHandlers.diagnosis.handleOnStartNewDiagnosis();
                    }
                );
            }
        },
        apiService: {
            getIllnesses: async function () {
                return await apiFetch(URLS.getIllnesses);
            },
            getSymptomsByIllnessId: async function (illnessId) {
                return await apiFetch(
                    URLS.getSymptomsByIllnessId,
                    {
                        params: {
                            IllnessId: illnessId
                        }
                    }
                );
            },
            runDiagnosis: async function (command) {
                return await apiFetch(
                    URLS.runDiagnosis,
                    {
                        method: "POST",
                        body: command
                    }
                )
            }
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();