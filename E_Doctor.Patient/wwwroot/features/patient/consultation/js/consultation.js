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
                    $(modals.symptoms.title).text(stateHolders.symptomModalTitle ?? "");
                },

                toggleDiagnosisResult: function (flag = false) {
                    const { modals } = elementHolders;
                    $(modals.diagnosis.root).toggleClass("visible", flag);
                },
            },
            illness: {
                setCommonSymptoms: async function () {
                    const symptomResult = await services.apiService.getSymptomsByIllnessId(null) ?? [];

                    //stateHolders.currentIllnessId = item.illnessId;
                    stateHolders.symptomModalTitle = "Please Select symptoms";

                    stateHolders.symptoms = symptomResult.map(item => {
                        const obj = { ...item, active: false, selectType: SELECTION_TYPES.NONE };
                        return obj;
                    });
                    stateHolders.currentSymptomIndex = 0;
                    services.eventHandlers.modal.toggleSymptom(true);
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
                handleOnShowIllness: function () {
                    const handlers = services.eventHandlers;
                    handlers.modal.toggleIllness(true);
                    this.populateIllnessList();
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
                handleOnNext: function () {
                    if (stateHolders.currentSymptomIndex == stateHolders.symptoms.length) return;
                    stateHolders.currentSymptomIndex += 1;

                    this.populateSymptomQuestion(1);
                },
                handleOnBack: function () {
                    if (stateHolders.currentSymptomIndex == 0) return;
                    stateHolders.currentSymptomIndex -= 1;

                    this.populateSymptomQuestion(-1);
                },
                populateSymptomQuestion: function (direction = 0) {
                    const { question, buttons, questionContent } = this.elements;
                    const symptom = stateHolders.symptoms[stateHolders.currentSymptomIndex];

                    if (!symptom) return;

                    const $questionContentElmt = $(questionContent);
                    $questionContentElmt.empty();


                    let animateDirectionClass = "";

                    if (direction != 0) {
                        animateDirectionClass = (direction == 1) ? "move-right" : "move-left"
                    }

                    const $card = $(`
                        <div class="symptom-card-container text-center ${animateDirectionClass}">
                            <p class="text-xl">${symptom.symptomName}</p>
                            <div class="flex symptom-yes-no-buttons">
                                <button class="btn btn-danger no-button yes-no-button" id="quest-no-btn">No</button>
                                <button class="btn btn-secondary yes-button yes-no-button" id="quest-yes-btn">Yes</button>
                            </div>
                        </div>
                    `);

                    setTimeout(() => {
                        $card.removeClass("move-right");
                        $card.removeClass("move-left");
                    }, 5);

                    $questionContentElmt.append($card);

                    const isLastItem = (stateHolders.symptoms.length - 1 == stateHolders.currentSymptomIndex);
                    const isFirstItem = (stateHolders.currentSymptomIndex == 0);

                    $(buttons.back).prop("disabled", isFirstItem);

                    $(buttons.next)
                        .prop("disabled", isLastItem);


                    $(this.elements.buttons.yesNo).removeClass("selected");

                    switch (symptom.selectType) {
                        case SELECTION_TYPES.NONE:
                            $(this.elements.buttons.yesNo).removeClass("selected");
                            break;
                        case SELECTION_TYPES.YES:
                            $(this.elements.buttons.yes).addClass("selected");
                            break;
                        case SELECTION_TYPES.NO:
                            $(this.elements.buttons.no).addClass("selected");
                            break;
                        default:
                            $(this.elements.buttons.yesNo).removeClass("selected");
                    };


                    registerEvent(
                        this.elements.buttons.yes,
                        "click",
                        function (e) {
                            services.eventHandlers.symptom.handleOnYes();
                        }
                    );

                    registerEvent(
                        this.elements.buttons.no,
                        "click",
                        function (e) {
                            services.eventHandlers.symptom.handleOnNo();
                        }
                    );
                },
                handleOnYes: function () {
                    const { buttons } = this.elements;

                    const symptom = stateHolders.symptoms[stateHolders.currentSymptomIndex];
                    if (!symptom) return;

                    stateHolders.symptoms[stateHolders.currentSymptomIndex].active = true;
                    stateHolders.symptoms[stateHolders.currentSymptomIndex].selectType = SELECTION_TYPES.YES;

                    $(this.elements.buttons.yesNo).removeClass("selected");
                    $(this.elements.buttons.yes).addClass("selected");

                    const hasSelected = stateHolders.symptoms.some(s => s.active);

                    $(buttons.getDiagnosis)
                        .prop("disabled", !hasSelected);
                },
                handleOnNo: function () {
                    const symptom = stateHolders.symptoms[stateHolders.currentSymptomIndex];
                    if (!symptom) return;

                    stateHolders.symptoms[stateHolders.currentSymptomIndex].active = false;
                    stateHolders.symptoms[stateHolders.currentSymptomIndex].selectType = SELECTION_TYPES.NO;

                    $(this.elements.buttons.yesNo).removeClass("selected");
                    $(this.elements.buttons.no).addClass("selected");
                },
                handleOnCancel: function () {
                    if (confirm("Are you sure to cancel diagnosis?")) {
                        stateHolders.symptoms = [];
                        stateHolders.currentSymptomIndex = 0;

                        services.eventHandlers.modal.toggleSymptom(false);
                        services.eventHandlers.modal.toggleIllness(false);
                    }
                }
            },
            diagnosis: {
                elements: elementHolders.modals.diagnosis,
                handleOnGetDiagnosis: async function () {
                    const hasSelectedItem = stateHolders.symptoms.filter(item => item.active == true).length > 0;
                    const illnessId = stateHolders.currentIllnessId;

                    if (!hasSelectedItem) {
                        alert("No symptom selected.\nPlease select at least 1 symptom.");
                        return;
                    }

                    const selectedSymptoms = stateHolders.symptoms
                        .filter(item => item.active === true)
                        .map(item => parseInt(item.symptomId));

                    const command = {
                        symptomIds: selectedSymptoms
                    }

                    const result = await services.apiService.runDiagnosis(command);

                    if (result) {
                        this.populateDiagnosisResult(result);
                    }

                },
                populateDiagnosisResult: function ({ result, description, prescription, notes, isSuccess }) {
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
                }
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
                        eventHandlers.illness.setCommonSymptoms();
                        //eventHandlers.illness.handleOnShowIllness();
                    }
                );

                registerEvent(
                    modals.illness.buttons.close,
                    "click",
                    function (e) {
                        eventHandlers.modal.toggleIllness(false);
                        stateHolders.currentIllnessId = null;
                    }
                );

                registerEvent(
                    modals.illness.search,
                    "input",
                    function (e) {
                        eventHandlers.illness.handleOnSearchIllness(e.target.value);
                    }
                );

                // symptoms
                registerEvent(
                    modals.symptoms.buttons.next,
                    "click",
                    function (e) {
                        eventHandlers.symptom.handleOnNext();
                    }
                );
                registerEvent(
                    modals.symptoms.buttons.back,
                    "click",
                    function (e) {
                        eventHandlers.symptom.handleOnBack();
                    }
                );
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