import { generate } from "rxjs";

export class ApiHelper {

  static readonly auth = {
    base: 'Auth',
    login: 'login',
    verifyOtp: 'verify-otp',
    refreshToken: 'refresh-token',
    resetPassword:'reset-password'
  };

  static readonly user = {
    base: 'Users',
    getAllUser: 'get-all',
    getUserById: '',
    add: 'create',
    update: 'update',
    delete: 'delete',
    getAllActiveSalesPerson: 'get-all-active-sales-person',
    getAllActiveUsers : 'get-all-active-users',
    getAllActiveDoctors: 'get-all-active-doctors',
    deactivateBulk: 'deactivate-bulk',
    activateBulk: 'activate-bulk',
    deleteBulk: 'delete-bulk',
    getActiveUsers : 'get-active-users',
    getAllVacationUsers: 'vacation-users'
  };

  static readonly clinicService = {
    base: 'ClinicService',
    getAllServiceByType: 'get-all-service-by-type',
    getAllServiceTypes:'get-all-appointment-services'
  };

  static readonly product = {
    base: 'Products',
    getAllProducts: 'get-all-products',
    getAllActiveProducts: 'get-all-active-products',
    publish: 'publish',
    deActivate: 'deactivate',
    delete: 'delete',
    getProductTypes: 'get-product-types',
    getProductCategories: 'get-product-categories',
    getProductWebForms: 'get-product-webforms',
    createProduct: 'create',
    getProductById: 'get-product-by-id',
    updateProduct: 'update',
    getAllProductsForDropdown: 'get-all-products-for-dropdown'
  };

  static readonly productStrength = {
    base: 'ProductStrength',
    getAllStrengthsByProductId: 'get-all-strength-by-productid',
    createProductStrength: 'create',
    updateProductStrength: 'update',
  };

  static readonly patient = {
    base: "Patient",
    getAllPatient: "get-all-patients",
    getAllActiveVisitType: 'get-all-active-visit-types',
    getAllActiveDocumentCategory: 'get-all-active-document-category',
    getAllActiveAgenda: 'get-all-active-agendas',
    getAllActivePatient: 'get-all-active-patients',
    getAllActiveDoctors: 'get-all-active-doctors',
    getUserById: '',
    add: 'create-patient',
    update: 'update',
    delete: 'delete',
    softDelete: 'soft-delete',
    deactivateBulk: 'deactivate-bulk',
    activateBulk: 'activate-bulk',
    activate: 'activate',
    deactivate: 'deactivate',
    bulkAssignee : 'bulk-assign',
    getAllCounselorBasedOnPatientId: 'get-all-active-notes-patientId',
    getCreditCardByPatientId : 'get-credit-cards-by-patient',
    getPatientsByCounselorId : 'patients-by-counselor',
    getPatientCounselorinfo:'get-patient-counselor-info',
    getPhysianInfoBasedOnPatientId : 'get-patient-physician-info'

  };
  static readonly pharmacy = {
    base: 'Pharmacy',
    getAllPharmacy: 'get-all-pharmacy',
    delete: 'delete',
    getPharmacyById : 'get-pharmacy-by-id',
    getAllCurrencies: 'get-all-currencies',
    activatePharmacy: 'activate',
    deactivatePharmacy: 'deactivate',
    createPharmacy: 'create',
    updatePharmacy: 'update',
    getAllPharmaciesForDropdown: 'get-all-pharmacy-for-dropdown',
    getAllShippingMethods: 'get-all-shipping-methods',
    getAllPharmacyShippingMethods : 'get-all-pharmacy-shipping-methods',
    getAllActivePharmacies: 'get-all-active-pharmacies',
  };
  static readonly priceListItems = {
    base: 'PriceListItem',
    getAllPriceListItems: 'get-all-pricelistitems',
    getAllPriceListItemsByProductId: 'get-pricelistitem-by-productid',
    getAllPriceListItemsByPharmacyId: 'get-pricelistitem-by-pharmacyid',
    getAllActivePriceListItemsByPharmacyId: 'get-active-pricelist-item-by-pharmacyid',
    activatePriceListItems: 'activate',
    deactivatePriceListItems: 'deactivate',
    deletePriceListItems: 'delete',
    getPriceListItemById: 'get-pricelistitem-by-id',
    getAllLifeFileDrugForms: 'get-all-lifefiledrugforms',
    getAllLifeFileQuantityUnits: 'get-all-lifefilequantityunits',
    getAllLifeFileScheduleCodes: 'get-all-lifefileschedulecodes',
    create: 'create',
    update: 'update',
  };
  static readonly commisionRates = {
    base: 'CommisionRate',
    getAllCommisionRates: 'get-all-commissionrate',
    getAllCommisionRatesByProductId: 'get-commissionrate-by-productId',
    activateCommisionRates: 'activate',
    deactivateCommisionRates: 'deactivate',
    deleteCommisionRates: 'delete',
    getCommisionRateById: 'get-commissionrate-by-id',
    create: 'create',
    update: 'update'
  };
  static readonly lead = {
    base: 'Lead',
    getAllLeads: 'get-all-leads',
    createLead : 'create-lead',
    updateLead: 'update-lead',
    delete: 'delete',
    getById : '',
    deactivateBulk: 'bulk-toggle-inActive',
    activateBulk: 'bulk-toggle-active',
    bulkAssignee : 'bulk-assign',
    bulkConvertToPatient: 'convert-to-patient',
    disqualifyLead: 'disqualify-lead',
    leadsByCounselorId : 'leads-by-counselor',
  };

  static readonly counselorNote = {
    base: 'CounselorNote',
    getAllCounselorBasedOnPatientId: 'get-all-active-notes-patientId',
    add: 'create',
    deleteNotes: 'delete',

  };
  static readonly medicalRecommendation = {
    base : 'MedicalRecommendation',
    get : 'get',
    getAllMedicationType: 'get-all-medication-type',
    getAllFollowUpTests: 'get-all-follow-up-tests',
    create : 'create',
    update : 'update',
    getAllBasedOnPatientId : 'get-all-by-patientId',
    delete: 'delete',
  }
  static readonly documentUpload = {
    base: 'PatientAttachment',
    uploadDocument: 'upload-documents',
    getAllDocuments: 'get-all-documents',
    getDocumentId : 'get-document-by-id',
    deleteDocument: 'delete-documents',
    //downloadDocument: 'download-documents',
  };
  static readonly coupon = {
  base: 'Coupon',
  getAllCoupons: 'get-all',
  getCouponById: 'get-coupon-by-id',
  createCoupon: 'create',
  updateCoupon: 'update',
  deleteCoupon: 'delete',
  activateCoupon: 'bulk-toggle-active',
  deactivateCoupon: 'bulk-toggle-inactive',
  delete : 'bulk-delete',
  getCouponsForDropdown: 'get-all-coupons-for-dropdown',
  getCoupons: 'get-coupons'
};
  static readonly proposal = {
    base : 'Proposal',
    getAllPropsals : 'get-all-proposals',
    getAllPropsalsOnPatientId : 'get-all-proposals-patientId',
    create : 'create',
    get : 'get',
    update : 'update',
    delete : 'delete',
    bulkAssignee : 'bulk-assign',
    bulkAssignAdmin : 'assign',
    acceptProposal : 'accept-proposal',
    rejectProposal : 'reject-proposal',
    cancelProposal : 'cancel-proposal',
    rejectOrderNoMoney : 'reject-order-nomoney',
    reject : 'reject-order',
    acceptByPatientProposal : 'accept-by-patient-proposal',
    rejectByPatientProposal : 'reject-by-patient-proposal',
    clone  : 'clone',
    updateProposalDetails : 'update-proposal-details',
    getCounselorInfo : 'get-counselor-info-on-patientId'

  };
    static readonly shippingAddress = {
    base : 'ShippingAddress',
    getAllShippingAddressBasedOnPatientId : 'get-all',
    getShippingAddressById : 'get-shipping-address-by-id',
    create : 'create',
    update : 'update',
    bulkActive : 'bulk-toggle-active',
    bulkInactive : 'bulk-toggle-inActive',
    bulkDelete : 'bulk-delete',
    getAllCountries : 'get-all-active-countries',
    getAllStatesByCountryId : 'get-all-active-states-by-countryId',
  };
  static readonly order = {
    base : 'Order',
    getAll : 'get-all',
    getById : 'get-by-id',
    create : 'create',
    update : 'update',
    delete : 'delete',
    accept : 'accept-order',
    getReceiptByOrderId : 'receipt-by-order',
    getPrescriptionByOrderId : 'prescription-by-order',
    getSignedPrescriptionByOrderId : 'signed-prescription-by-order',
    markReadyToLifeFile : 'mark-ready-to-lifefile',
    generateCommission : 'generate-commission',
    getAllOrderprocessingApiTrackingErrors : 'get-all-orderprocessing-api-tracking-errors',
    updatePayment: 'update-payment',
    cancelGenerateCommission : 'cancel-commission',
    processRefund : 'refund',
    settleOutstandingRefund : 'settle-outstanding-refund'

  }

  static readonly appointment = {
    base: 'Appointment',
    getAllAppointments: 'get-all-appointments',
    deleteAppointment:'delete',
    getAllAppointmentModes:'get-all-appointmentmodes',
    getAllSlots:'get-all-slots',
    create:'create',
    update:'update',
    getById:'get',
    getAppointmentsByPatientId: 'get-by-patientId'
  };
  static readonly holiday = {
    base: 'Holiday',
    create: 'create',
    getAll:'all'
  };
  static readonly timezone = {
    base: 'Timezone',
    getAll:'get-all'
  };
  static readonly conversation = {
    base: 'Conversation',
    getConversationByPatientId : 'getConversationByPatientId',
    getConversationByLeadId : 'getConversationByLeadId',
    create : 'create',
    getUnReadConversationByCounselorId : 'get-unread-conversation-by-counselorId',
    markMessagesAsRead : 'mark-messages-as-read'
  }
  static readonly batchMessage = {
    base: 'BatchMessage',
    getById : '',
    getAll: 'get-all',
    create: 'create',
    update: 'update',
    approve : 'approve',
    reject : 'reject',
    delete : 'delete',

  }
  static readonly hub = {
    sms: 'smshub',
  }
  static readonly pharmacyConfiguration = {
    base: 'PharmacyConfiguration',
    getAll:'get-all-configurations',
    activate: 'activate',
    deactivate: 'deactivate',
    delete: 'delete'
  };
  static readonly reminder = {
    base: 'Reminder',
    getAllReminderTypes: 'types',
    getAllRecurrenceRules: 'recurrence-rules',
    create: 'create',
    allActiveRemindersForPatient: 'patient/all',
    allActiveRemindersForLeads: 'lead/all',
    markReminderasCompleted: 'mark-completed'
  };
  static readonly commission = {
    base: 'Commission',
    getCounselorByDate:'get-counselors-by-date',
    getCommissionsByPoolDetailId: 'get-commission-by-poolDetailId',
    getCommissionById: 'get-commission-by-getCommissionByIdAsync',
  }
  static readonly orderProductRefill = {
    base: 'OrderProductsRefill',
    getAllOrderProductsRefillDetails:'getAllOrderProductsRefill',
    delete: 'delete',
    getById: 'getOrderProductRefillById',
    update: 'update'
  }
  static readonly orderProductSchedule = {
    base: 'OrderProductSchedule',
    getAllOrderProductsScheduleDetails:'filter',
    schedulesSummary: 'patient/schedules/summary',
    updateScheduleTime: 'update-schedule-time',
    schedulesSummaryById: 'summary',
    updateScheduleSummary: 'summary',
    createSelfReminderForPatient: 'patient-self-reminder',
    getPatientSelfReminders:'patient-self-reminder/filter'
  }
}
