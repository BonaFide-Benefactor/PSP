import { ILeasePage } from 'features/leases';
import { apiLeaseToFormLease } from 'features/leases/leaseUtils';
import { Form, Formik } from 'formik';
import { defaultFormLease, IFormLease, ILease } from 'interfaces';
import queryString from 'query-string';
import * as React from 'react';
import { useLocation } from 'react-router-dom';
import styled from 'styled-components';

export interface ILeasePageFormProps {
  leasePage: ILeasePage;
  lease?: ILease;
  refreshLease: () => void;
  setLease: (lease: ILease) => void;
}

/**
 * Wraps the lease page in a formik form
 * @param {ILeasePageFormProps} param0
 */
export const LeasePageForm: React.FunctionComponent<
  React.PropsWithChildren<ILeasePageFormProps>
> = ({ leasePage, lease, refreshLease, setLease, children }) => {
  const location = useLocation();
  const { edit } = queryString.parse(location.search);

  return (
    <StyledLeasePage>
      <StyledLeasePageHeader>{leasePage.header}</StyledLeasePageHeader>
      {!edit ? (
        <Formik<IFormLease>
          initialValues={{ ...defaultFormLease, ...apiLeaseToFormLease(lease) }}
          enableReinitialize={true}
          validationSchema={leasePage?.validation}
          onSubmit={async (values, { setSubmitting }) => {
            try {
              refreshLease();
            } finally {
              setSubmitting(false);
            }
          }}
        >
          {formikProps => (
            <>
              <ViewEditToggleForm id="leaseForm">{children}</ViewEditToggleForm>
            </>
          )}
        </Formik>
      ) : (
        <>{children}</>
      )}
    </StyledLeasePage>
  );
};

export const ViewEditToggleForm = styled(Form)`
  &#leaseForm {
    text-align: left;
    input:disabled,
    select:disabled,
    textarea:disabled {
      background: none;
      border: none;
      resize: none;
      height: fit-content;
    }
    textarea:disabled {
      padding-left: 0;
    }
    .input-group.disabled {
      .input-group-text {
        background: none;
        border: none;
      }
    }
  }
`;

const StyledLeasePageHeader = styled.div`
  font-family: 'BCSans-Bold';
  font-size: 3.2rem;
  line-height: 4.2rem;
  text-align: left;
  color: ${props => props.theme.css.textColor};
  position: sticky;
  top: 0;
  left: 0;
  height: 1rem;
  padding-bottom: 1rem;
  background-color: #f2f2f2;

  z-index: 10;
`;

const StyledLeasePage = styled.div`
  height: 100%;
  overflow-y: auto;
  grid-area: leasecontent;
  flex-direction: column;
  display: flex;
  text-align: left;
  background-color: #f2f2f2;
`;

export default LeasePageForm;
