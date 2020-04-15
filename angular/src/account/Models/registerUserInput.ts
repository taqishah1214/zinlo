import { SubscriptionPlans } from "./SubscriptionPlans"
import { BusinessInfo } from "./BusinessInfo"
import { PaymentDetails } from "./PaymentDetails"
import { PersonalInfo } from "./PersonalInfo"

export class RegisterUserInput {
    businessInfo: BusinessInfo;
    paymentDetails: PaymentDetails;
    personalInfo: PersonalInfo;
    subscriptionPlans: SubscriptionPlans;
}