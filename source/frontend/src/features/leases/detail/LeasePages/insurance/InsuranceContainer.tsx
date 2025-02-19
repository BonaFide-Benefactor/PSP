import { CancelConfirmationModal } from 'components/common/CancelConfirmationModal';
import { INSURANCE_TYPES } from 'constants/API';
import { Claims } from 'constants/claims';
import { useLeaseDetail } from 'features/leases';
import { getIn } from 'formik';
import useKeycloakWrapper from 'hooks/useKeycloakWrapper';
import useLookupCodeHelpers from 'hooks/useLookupCodeHelpers';
import { IInsurance } from 'interfaces';
import queryString from 'query-string';
import React, { useState } from 'react';
import { useHistory, useLocation } from 'react-router-dom';

import InsuranceDetailsView from './details/Insurance';
import InsuranceEditContainer from './edit/EditContainer';

const InsuranceContainer: React.FunctionComponent<React.PropsWithChildren<unknown>> = () => {
  const { hasClaim } = useKeycloakWrapper();
  const [showCancelModal, setShowCancelModal] = useState(false);

  const { refresh, lease } = useLeaseDetail();
  const leaseId: number = getIn(lease, 'id') || -1;
  const insuranceList: IInsurance[] = getIn(lease, 'insurances') ?? [];

  const lookupCodes = useLookupCodeHelpers();
  const insuranceTypes = lookupCodes.getByType(INSURANCE_TYPES).sort((a, b) => {
    return (a.displayOrder || 0) - (b.displayOrder || 0);
  });
  const location = useLocation();
  const history = useHistory();
  const { edit } = queryString.parse(location.search);

  return (
    <>
      {!edit && (
        <InsuranceDetailsView insuranceList={insuranceList} insuranceTypes={insuranceTypes} />
      )}
      {edit && hasClaim(Claims.LEASE_EDIT) && (
        <InsuranceEditContainer
          leaseId={leaseId}
          insuranceList={insuranceList}
          insuranceTypes={insuranceTypes}
          onSuccess={async () => {
            await refresh();
            history.push(location.pathname);
          }}
          onCancel={(dirty?: boolean) => {
            if (dirty) {
              setShowCancelModal(true);
            } else {
              history.push(location.pathname);
            }
          }}
        />
      )}
      <CancelConfirmationModal
        display={showCancelModal}
        setDisplay={setShowCancelModal}
        handleOk={() => history.push(location.pathname)}
      />
    </>
  );
};

export default InsuranceContainer;
