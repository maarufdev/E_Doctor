const ILLNESS = [
    { illnessId: 1, illnessName: "Anxiety Disorder" },
    { illnessId: 2, illnessName: "Asthmar" },
    { illnessId: 3, illnessName: "Atopic Dermatitis (Eczema)" },
    { illnessId: 4, illnessName: "Bronchitis" },
    { illnessId: 5, illnessName: "Common Cold" },
    { illnessId: 6, illnessName: "Conjunctivitis (Pink Eye)" },

];

(function () {
    const SYMPTOM_BASE_URL = "";

    const URLS = {
    }
    const stateHolders = {
        currentIllnessId: null,
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
                buttons: {

                }
            }
        }
    }

    const services = {
        initialize: function () {
            services.events.initMain();
        },
        eventHandlers: {
            modal: {
                toggleIllness: function (flag = false) {
                    const { modals } = elementHolders;
                    $(modals.illness.root).toggleClass("visible", flag);
                },
            },
            illness: {
                pupulate: async function () {
                    const { modals } = elementHolders;
                    const $illnessList = $(modals.illness.illnessList);
                    $illnessList.empty();

                    const result = await services.apiService.getIllnesses();

                    result.forEach(item => {
                        const $illnessItem = $(`
                            <button class="btn illness-item-btn" title="${item.illnessName}" data-illness-id="${item.illnessId}">${item.illnessName}</button>
                        `);

                        registerEvent(
                            $illnessItem,
                            "click",
                            function () {
                                stateHolders.currentIllnessId = item.illnessId;
                                alert(stateHolders.currentIllnessId);
                            }
                        );

                        $illnessList.append($illnessItem);
                    })
                },
                handleOnShowIllness: function () {
                    const handlers = services.eventHandlers;
                    handlers.modal.toggleIllness(true);
                    this.pupulate();
                },
            }

        },
        events: {
            initMain: function () {
                const { buttons, modals } = elementHolders;

                registerEvent(
                    buttons.startNewConsultation,
                    "click",
                    function (e) {
                        services.eventHandlers.illness.handleOnShowIllness();
                    }
                );

                registerEvent(
                    modals.illness.buttons.close,
                    "click",
                    function (e) {
                        services.eventHandlers.modal.toggleIllness(false);
                        stateHolders.currentIllnessId = null;
                    }
                );

                registerEvent(
                    modals.illness.search,
                    "input",
                    function (e) {
                        console.log(e.target.value);
                    }
                );
            }
        },
        apiService: {
            getIllnesses: async function () {
                return ILLNESS;
            },
            getSymptomsByIllnessId: async function (illnessId) {

            }
        }
    }
    document.addEventListener("DOMContentLoaded", services.initialize);
})();