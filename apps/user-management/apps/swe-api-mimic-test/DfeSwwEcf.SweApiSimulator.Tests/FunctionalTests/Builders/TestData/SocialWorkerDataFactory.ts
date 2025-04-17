// Defining an interface for the expected response structure
interface ExpectedResponse {
    registeredName: string;
    registrationNumber: string;
    status: string;
    townOfEmployment?: string;
    registeredFrom: string;
    registeredUntil: string;
    registered: boolean;
}

// Defining an interface for the test data structure
interface SocialWorkerTestData {
    swid: number;
    expectedResponse: ExpectedResponse;
}

// Data factory class
export class SocialWorkerDataFactory {
    // Static method to create social worker test data
    static createSocialWorkerTestData(): SocialWorkerTestData[] {
        return [
            {
                swid: 2,
                expectedResponse: {
                    registeredName: 'Nelda Schroeder',
                    registrationNumber: 'SW2',
                    status: 'No Longer Registered',
                    townOfEmployment: '',
                    registeredFrom: '2012-08-01T00:00:00',
                    registeredUntil: '2016-12-01T00:00:00',
                    registered: false,
                },
            },
            {
                swid: 1,
                expectedResponse: {
                    registeredName: 'Lisa MacGyver',
                    registrationNumber: 'SW1',
                    status: 'No Longer Registered - Failed to Renew',
                    townOfEmployment: '',
                    registeredFrom: '2012-08-01T00:00:00',
                    registeredUntil: '2021-11-30T00:00:00',
                    registered: false,
                },
            },
            {
                swid: 1961,
                expectedResponse: {
                    registeredName: 'Aliya Rowe',
                    registrationNumber: 'SW1961',
                    status: 'No Longer Registered - Voluntary Removal',
                    townOfEmployment: '',
                    registeredFrom: '2012-08-01T00:00:00',
                    registeredUntil: '2022-11-30T00:00:00',
                    registered: false,
                },
            },
            {
                swid: 2107,
                expectedResponse: {
                    registeredName: 'Xander Davis',
                    registrationNumber: 'SW2107',
                    status: 'No Longer Registered - Deceased',
                    townOfEmployment: '',
                    registeredFrom: '2012-08-01T00:00:00',
                    registeredUntil: '2015-06-02T00:00:00',
                    registered: false,
                },
            },
            {
                swid: 4504,
                expectedResponse: {
                    registeredName: 'Heath Sauer',
                    registrationNumber: 'SW4504',
                    status: 'No Longer Registered - Failed to meet CPD requirements',
                    townOfEmployment: '',
                    registeredFrom: '2012-08-01T00:00:00',
                    registeredUntil: '2021-11-30T00:00:00',
                    registered: false,
                },
            },
            {
                swid: 9210,
                expectedResponse: {
                    registeredName: 'Candace Bosco',
                    registrationNumber: 'SW9210',
                    status: 'No Longer Registered - Removed',
                    townOfEmployment: '',
                    registeredFrom: '2012-08-01T00:00:00',
                    registeredUntil: '2018-02-05T00:00:00',
                    registered: false,
                },
            },
        ];
    }
}

