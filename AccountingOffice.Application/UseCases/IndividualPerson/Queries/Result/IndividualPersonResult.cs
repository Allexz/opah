namespace AccountingOffice.Application.UseCases.Individual.Queries.Result;

public sealed record IndividualPersonResult(Guid id,
                                            Guid tenantId,
                                            string name,
                                            string document,
                                            int type,
                                            string email,
                                            string phoneNumber,
                                            int maritalStatus);
