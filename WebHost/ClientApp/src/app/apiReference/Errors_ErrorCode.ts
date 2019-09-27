// ! Generated Code ! 

// ReSharper disable InconsistentNaming
export enum ErrorCode {
    /// 
    ValidationError = 0,
    /// 
    OperationForbidden = 1,
    /// 
    ObjectNotFound = 2

}

export module ErrorCode {
    export function getStringValuePresentation(value: ErrorCode): string {
        switch(value)
        {
            case ErrorCode.ValidationError:
                return "ValidationError";
            case ErrorCode.OperationForbidden:
                return "OperationForbidden";
            case ErrorCode.ObjectNotFound:
                return "ObjectNotFound";
        }

    }
}


// ReSharper restore InconsistentNaming

